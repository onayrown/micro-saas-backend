FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroSaaS.Backend/MicroSaaS.Backend.csproj", "MicroSaaS.Backend/"]
COPY ["MicroSaaS.Domain/MicroSaaS.Domain.csproj", "MicroSaaS.Domain/"]
COPY ["MicroSaaS.Shared/MicroSaaS.Shared.csproj", "MicroSaaS.Shared/"]
COPY ["MicroSaaS.Application/MicroSaaS.Application.csproj", "MicroSaaS.Application/"]
COPY ["MicroSaaS.Infrastructure/MicroSaaS.Infrastructure.csproj", "MicroSaaS.Infrastructure/"]
RUN dotnet restore "MicroSaaS.Backend/MicroSaaS.Backend.csproj"
COPY . .
WORKDIR "/src/MicroSaaS.Backend"
RUN dotnet build "MicroSaaS.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroSaaS.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroSaaS.Backend.dll"] 