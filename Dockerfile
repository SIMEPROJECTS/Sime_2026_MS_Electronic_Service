# Utiliza el SDK de .NET 8.0 para la etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia los archivos de proyecto y restaura las dependencias
COPY MicroservicesEcosystem/*.csproj ./MicroservicesEcosystem/
RUN dotnet restore MicroservicesEcosystem/*.csproj -r linux-x64

# Copia el resto de los archivos y construye la aplicación
COPY MicroservicesEcosystem/. ./MicroservicesEcosystem/
WORKDIR /source/MicroservicesEcosystem
RUN dotnet publish -c Release -o /app -r linux-x64 --self-contained false --no-restore

# Etapa final, utiliza la imagen de ASP.NET Core Runtime 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0

ENV TZ=America/Guayaquil
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
 && echo $TZ > /etc/timezone
 
WORKDIR /app
EXPOSE 80
COPY --from=build /app ./

# 🔴 ESTE ES EL OTRO CAMBIO NECESARIO
ENTRYPOINT ["dotnet", "MicroservicesEcosystem.dll"]
