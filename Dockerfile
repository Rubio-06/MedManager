FROM node:20-alpine AS node-build
WORKDIR /web

# Cache npm dependencies first.
COPY src/MedManager.Web/package*.json ./
RUN if [ -f package-lock.json ]; then npm ci --no-audit --no-fund; else npm install --no-audit --no-fund; fi

COPY src/MedManager.Web/ ./
RUN npm run css:build


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first to maximize restore cache.
COPY MedManagr.sln ./
COPY src/MedManager.Web/MedManager.Web.csproj src/MedManager.Web/
COPY src/MedManager.Application/MedManager.Application.csproj src/MedManager.Application/
COPY src/MedManager.Domain/MedManager.Domain.csproj src/MedManager.Domain/
COPY src/MedManager.Infrastructure/MedManager.Infrastructure.csproj src/MedManager.Infrastructure/

RUN dotnet restore src/MedManager.Web/MedManager.Web.csproj

# Copy remaining sources.
COPY src/ ./src/

# Inject built Tailwind assets from Node stage.
COPY --from=node-build /web/wwwroot/css/style.css /src/src/MedManager.Web/wwwroot/css/style.css

RUN dotnet publish src/MedManager.Web/MedManager.Web.csproj \
    -c Release \
    -o /app/publish \
    /p:SkipTailwindBuild=true


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MedManager.Web.dll"]