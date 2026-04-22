# =========================
# 1. NODE (Tailwind build)
# =========================
FROM node:20 AS node-build

WORKDIR /web

# Copie uniquement package.json pour cache npm
COPY src/MedManager.Web/package*.json ./
RUN npm install

# Copie tout le frontend
COPY src/MedManager.Web ./

# Build Tailwind / CSS
RUN npm run css:build


# =========================
# 2. DOTNET BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY src ./src

# Restore
RUN dotnet restore src/MedManager.Web/MedManager.Web.csproj

# Publish (sans npm ici !)
RUN dotnet publish src/MedManager.Web/MedManager.Web.csproj \
    -c Release \
    -o /app/publish


# =========================
# 3. RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .
COPY --from=node-build /web/wwwroot ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "MedManager.Web.dll"]