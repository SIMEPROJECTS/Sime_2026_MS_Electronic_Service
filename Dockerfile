# Utiliza el SDK de .NET 8.0 para la etapa de construcción
# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar solo el csproj
COPY MicroservicesEcosystem/*.csproj ./MicroservicesEcosystem/

# Restore (YA NO FALLA)
RUN dotnet restore MicroservicesEcosystem/*.csproj -r linux-x64

# Copiar el resto del código
COPY MicroservicesEcosystem/. ./MicroservicesEcosystem/

# Publish
WORKDIR /source/MicroservicesEcosystem
RUN dotnet publish \
    -c Release \
    -o /app/publish \
    -r linux-x64 \
    --self-contained false \
    --no-restore

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV TZ=America/Guayaquil
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
 && echo $TZ > /etc/timezone

WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MicroservicesEcosystem.dll"]
