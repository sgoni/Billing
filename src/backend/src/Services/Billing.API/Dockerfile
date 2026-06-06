FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Billing.API/Billing.API.csproj", "src/backend/src/Services/Billing.API/"]
COPY ["src/shared/BuildingBlocks.Secrets/BuildingBlocks.Secrets.csproj", "src/backend/src/shared/BuildingBlocks.Secrets/"]
COPY ["src/shared/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "src/backend/src/shared/BuildingBlocks.Messaging/"]
COPY ["src/shared/BuildingBlocks.Logging/BuildingBlocks.Logging.csproj", "src/backend/src/shared/BuildingBlocks.Logging/"]
COPY ["src/shared/BuildingBlocks.Discovery/BuildingBlocks.Discovery.csproj", "src/backend/src/shared/BuildingBlocks.Discovery/"]
COPY ["src/Services/Billing.Domain/Billing.Domain.csproj", "src/backend/src/Services/Billing.Domain/"]
COPY ["src/shared/BuildingBlocks/BuildingBlocks.csproj", "src/backend/src/shared/BuildingBlocks/"]
COPY ["src/Services/Billing.Application/Billing.Application.csproj", "src/backend/src/Services/Billing.Application/"]
COPY ["src/Services/Billing.Infrastructure/Billing.Infrastructure.csproj", "src/backend/src/Services/Billing.Infrastructure/"]
RUN dotnet restore "src/backend/src/Services/Billing.API/Billing.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Billing.API"
RUN dotnet build "./Billing.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Billing.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Billing.API.dll"]
