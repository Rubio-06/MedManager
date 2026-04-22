# =========================
# 1. Frontend (Tailwind)
# =========================
FROM node:20 AS node-build
WORKDIR /frontend

# Copie uniquement les fichiers npm en premier (cache Docker optimisé)
COPY src/MedManager.Web/package*.json ./

RUN npm install

# Copie le reste du frontend
COPY src/MedManager.Web ./

# Build Tailwind / CSS
RUN npm run css:build


# =========================
# 2. Build .NET
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src ./src

# (optionnel mais recommandé)
RUN dotnet restore src/MedManager.Web/MedManager.Web.csproj

RUN dotnet publish src/MedManager.Web/MedManager.Web.csproj \
    -c Release \
    -o /app/publish


# =========================
# 3. Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# 👇 on injecte le CSS compilé Tailwind dans le publish
COPY --from=node-build /frontend/wwwroot ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "MedManager.Web.dll"]