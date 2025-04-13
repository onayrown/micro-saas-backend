FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define o diretório de trabalho
WORKDIR /src

# Copiar arquivos do projeto para restaurar dependências
COPY ["MicroSaaS.Backend/MicroSaaS.Backend.csproj", "MicroSaaS.Backend/"]
COPY ["MicroSaaS.Application/MicroSaaS.Application.csproj", "MicroSaaS.Application/"]
COPY ["MicroSaaS.Domain/MicroSaaS.Domain.csproj", "MicroSaaS.Domain/"]
COPY ["MicroSaaS.Infrastructure/MicroSaaS.Infrastructure.csproj", "MicroSaaS.Infrastructure/"]
COPY ["MicroSaaS.Shared/MicroSaaS.Shared.csproj", "MicroSaaS.Shared/"]

# Restaurar pacotes NuGet
RUN dotnet restore "MicroSaaS.Backend/MicroSaaS.Backend.csproj"

# Copiar o restante dos arquivos de origem
COPY . .

# Compilar o projeto
WORKDIR /src
RUN dotnet build "MicroSaaS.Backend/MicroSaaS.Backend.csproj" -c Release -o /app/build

FROM build AS publish
# Publicar o projeto
WORKDIR /src
RUN dotnet publish "MicroSaaS.Backend/MicroSaaS.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copiar da etapa de publicação para a imagem final
COPY --from=publish /app/publish .

# Criar arquivo .env vazio se não existir
RUN touch .env 

# Definir ponto de entrada para a aplicação
ENTRYPOINT ["dotnet", "MicroSaaS.Backend.dll"] 