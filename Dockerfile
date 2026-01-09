# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar solución y proyectos
COPY *.sln .
COPY MicroservicesEcosystem/*.csproj ./MicroservicesEcosystem/

# Restaurar dependencias
RUN dotnet restore *.sln -r linux-x64

# Copiar el resto del código
COPY MicroservicesEcosystem/. ./MicroservicesEcosystem/

# Publicar
WORKDIR /source/MicroservicesEcosystem
RUN dotnet publish \
    -c Release \
    -o /app/publish \
    -r linux-x64 \
    --self-contained false \
    --no-restore

# Etapa runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV TZ=America/Guayaquil
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
 && echo $TZ > /etc/timezone

WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MicroservicesEcosystem.dll"]
