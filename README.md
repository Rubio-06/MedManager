# MedManager

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
