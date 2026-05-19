FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Workers/Integration.Services/Integration.Services.csproj", "src/backend/src/Workers/Integration.Services/"]
COPY ["src/shared/BuildingBlocks/BuildingBlocks.csproj", "src/backend/src/shared/BuildingBlocks/"]
COPY ["src/shared/BuildingBlocks.Logging/BuildingBlocks.Logging.csproj", "src/backend/src/shared/BuildingBlocks.Logging/"]
COPY ["src/shared/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "src/backend/src/shared/BuildingBlocks.Messaging/"]
RUN dotnet restore "src/backend/src/Workers/Integration.Services/Integration.Services.csproj"
COPY . .
WORKDIR "/src/src/Workers/Integration.Services"
RUN dotnet build "./Integration.Services.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Integration.Services.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Integration.Services.dll"]
