# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Primeiro copiamos os arquivos de projeto para restaurar as dependências
COPY ["MicroSaaS.Backend.csproj", "MicroSaaS.Backend/"]
COPY ["../MicroSaaS.Domain/MicroSaaS.Domain.csproj", "MicroSaaS.Domain/"]
COPY ["../MicroSaaS.Shared/MicroSaaS.Shared.csproj", "MicroSaaS.Shared/"]
COPY ["../MicroSaaS.Application/MicroSaaS.Application.csproj", "MicroSaaS.Application/"]
COPY ["../MicroSaaS.Infrastructure/MicroSaaS.Infrastructure.csproj", "MicroSaaS.Infrastructure/"]

# Restaurar as dependências
RUN dotnet restore "MicroSaaS.Backend/MicroSaaS.Backend.csproj"

# Copiar os arquivos fonte
COPY . "MicroSaaS.Backend/"
COPY ../MicroSaaS.Domain/ "MicroSaaS.Domain/"
COPY ../MicroSaaS.Shared/ "MicroSaaS.Shared/"
COPY ../MicroSaaS.Application/ "MicroSaaS.Application/"
COPY ../MicroSaaS.Infrastructure/ "MicroSaaS.Infrastructure/"

# Construir o projeto
WORKDIR "/src/MicroSaaS.Backend"
RUN dotnet build "MicroSaaS.Backend.csproj" -c Release -o /app/build

# Stage de publicação
FROM build AS publish
RUN dotnet publish "MicroSaaS.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroSaaS.Backend.dll"]