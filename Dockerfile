FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src ./src

RUN dotnet restore src/MedManager.Web/MedManager.Web.csproj

RUN dotnet publish src/MedManager.Web/MedManager.Web.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "MedManager.Web.dll"]