FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Workers/IntegrationService/IntegrationService.csproj", "src/backend/src/Workers/IntegrationService/"]
COPY ["src/shared/BuildingBlocks/BuildingBlocks.csproj", "src/backend/src/shared/BuildingBlocks/"]
COPY ["src/shared/BuildingBlocks.Logging/BuildingBlocks.Logging.csproj", "src/backend/src/shared/BuildingBlocks.Logging/"]
COPY ["src/shared/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "src/backend/src/shared/BuildingBlocks.Messaging/"]
RUN dotnet restore "src/backend/src/Workers/IntegrationService/IntegrationService.csproj"
COPY . .
WORKDIR "/src/src/backend/Workers/IntegrationService"
RUN dotnet build "./IntegrationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IntegrationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntegrationService.dll"]
