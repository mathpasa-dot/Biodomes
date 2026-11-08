# syntax=docker/dockerfile:1

# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restauration des paquets NuGet en premier (mise en cache tant que les .csproj ne changent pas)
COPY global.json ./
COPY BioDomes.Domains/BioDomes.Domains.csproj BioDomes.Domains/
COPY BioDomes.Infrastructures/BioDomes.Infrastructures.csproj BioDomes.Infrastructures/
COPY BioDomes.Web/BioDomes.Web.csproj BioDomes.Web/
RUN dotnet restore BioDomes.Web/BioDomes.Web.csproj

COPY BioDomes.Domains/ BioDomes.Domains/
COPY BioDomes.Infrastructures/ BioDomes.Infrastructures/
COPY BioDomes.Web/ BioDomes.Web/
RUN dotnet publish BioDomes.Web/BioDomes.Web.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DataDirectory=/app/data

COPY --from=build --chown=app:app /app/publish .
RUN mkdir -p /app/data && chown app:app /app/data

# Utilisateur non-root fourni par les images .NET 8
USER app

# /app/data            : base SQLite + clés Data Protection
# /app/wwwroot/images  : images fournies + uploads (espèces, équipements, avatars)
VOLUME ["/app/data", "/app/wwwroot/images"]

EXPOSE 8080
ENTRYPOINT ["dotnet", "BioDomes.Web.dll"]
