# MedManager Refactor

MedManager est une application web pour gérer des patients, des médecins, des médicaments, des allergies et des prescriptions.

## Mise en production

Cette application peut être installée sur n’importe quel serveur qui supporte Docker. Ce n’est pas lié à Coolify: tu peux l’utiliser sur un VPS, un serveur privé ou une machine dédiée.

### Ce qu’il faut

- Docker
- Docker Compose
- Une base MySQL ou MariaDB

### Étape 1: récupérer le projet

```bash
git clone <url-du-repo>
cd MedManager
```

### Étape 2: créer le fichier `.env`

Copie [.env.example](.env.example) en `.env` puis modifie la chaîne de connexion.

Exemple simple:

```text
CONNECTION_STRING=server=adresse-de-ta-base;port=3306;database=medmanager;user=medmanager;password=motdepasse
```

Si ta base est sur le même serveur ou dans un autre conteneur, remplace juste `adresse-de-ta-base` par le bon nom d’hôte ou l’adresse IP.

### Étape 3: lancer l’application

```bash
docker compose up -d --build
```

## Exemple avec une base locale dans Docker

Si tu ne veux pas utiliser une base externe, tu peux lancer aussi une base MySQL ou MariaDB dans le même `docker compose`.

Exemple simple de service DB:

```yaml
services:
	web:
		image: ton-image-medmanager
		environment:
			CONNECTION_STRING: server=db;port=3306;database=medmanager;user=medmanager;password=motdepasse
		depends_on:
			- db

	db:
		image: mariadb:11
		environment:
			MYSQL_ROOT_PASSWORD: motdepasse
			MYSQL_DATABASE: medmanager
			MYSQL_USER: medmanager
			MYSQL_PASSWORD: motdepasse
		volumes:
			- mysql_data:/var/lib/mysql

volumes:
	mysql_data:
```

Dans ce cas, le nom d’hôte de la base est `db` parce que c’est le nom du service Docker.

## Variables importantes

- `CONNECTION_STRING` : obligatoire, c’est la connexion à la base de données
- `ASPNETCORE_ENVIRONMENT` : mets `Production` sur un serveur
- `DB_INIT_MAX_RETRIES` : nombre de tentatives de connexion au démarrage
- `DB_INIT_RETRY_DELAY_SECONDS` : délai entre deux tentatives
- `SEED_DEMO_USERS` : met `true` si tu veux les comptes de démo en production
- `CLEANUP_LEGACY_DEMO_USERS` : met `true` pour supprimer les anciens comptes de démo

## Comptes de démo

Si le seed de démo est activé, tu auras :

- Admin: `admin@medmanager.com` / `Admin123!`
- Doctor: `doctor@medmanager.com` / `Doctor123!`
- Patient: `patient@medmanager.com` / `Patient123!`

Le seed ajoute aussi les allergies et les médicaments de démonstration.

## Créer un admin manuellement en SQL

Si tu veux partir de zero sans seed, tu peux créer un compte admin a la main.

Important: `PasswordHash` ne peut pas etre un mot de passe en clair. Il faut un hash ASP.NET Identity.

### 1) Generer un hash de mot de passe

Le plus simple est d'utiliser temporairement le seed (`SEED_DEMO_USERS=true`), puis de copier la valeur `PasswordHash` de l'utilisateur admin depuis `AspNetUsers`.

Exemple pour lire le hash actuel:

```sql
SELECT Id, Email, PasswordHash
FROM AspNetUsers
WHERE Email = 'admin@medmanager.com';
```

### 2) Requete SQL pour creer le role + user + liaison admin

Remplace la valeur `__PASSWORD_HASH__` avant execution.

```sql
START TRANSACTION;

-- 1) Role Admin
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
SELECT UUID(), 'Admin', 'ADMIN', UUID()
WHERE NOT EXISTS (
	SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN'
);

SET @roleId = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN' LIMIT 1);

-- 2) User admin
SET @newUserId = UUID();

INSERT INTO AspNetUsers (
	Id,
	UserName,
	NormalizedUserName,
	Email,
	NormalizedEmail,
	EmailConfirmed,
	PasswordHash,
	SecurityStamp,
	ConcurrencyStamp,
	PhoneNumberConfirmed,
	TwoFactorEnabled,
	LockoutEnabled,
	AccessFailedCount
)
SELECT
	@newUserId,
	'admin@medmanager.com',
	'ADMIN@MEDMANAGER.COM',
	'admin@medmanager.com',
	'ADMIN@MEDMANAGER.COM',
	1,
	'__PASSWORD_HASH__',
	UUID(),
	UUID(),
	0,
	0,
	1,
	0
WHERE NOT EXISTS (
	SELECT 1 FROM AspNetUsers WHERE NormalizedEmail = 'ADMIN@MEDMANAGER.COM'
);

SET @userId = (SELECT Id FROM AspNetUsers WHERE NormalizedEmail = 'ADMIN@MEDMANAGER.COM' LIMIT 1);

-- 3) Lien user-role
INSERT IGNORE INTO AspNetUserRoles (UserId, RoleId)
VALUES (@userId, @roleId);

-- 4) Entite metier Admin dans Persons (TPH)
INSERT INTO Persons (
	FirstName,
	LastName,
	ApplicationUserId,
	PersonType
)
SELECT 'Admin', 'System', @userId, 'Admin'
WHERE NOT EXISTS (
	SELECT 1 FROM Persons WHERE ApplicationUserId = @userId
);

COMMIT;
```

Apres execution, connecte-toi avec l'email admin et le mot de passe correspondant au hash que tu as utilise.

## Lancer en local sans Docker

Si tu veux tester en local:

```bash
cd src/MedManager.Web
npm install
npm run css:build
dotnet run
```

## Résolution de problèmes

Si le site s’ouvre mais ne se connecte pas à la base:

- vérifie `CONNECTION_STRING`
- vérifie le nom du serveur de base
- vérifie le mot de passe
- vérifie que la base est bien démarrée

Si tu veux désactiver les comptes de démo:

```text
SEED_DEMO_USERS=false
```
