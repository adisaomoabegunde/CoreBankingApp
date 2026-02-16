# ── Base runtime image ──
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ── Build stage ──
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first for better layer caching
COPY ["CoreBanking.Api/CoreBanking.Api.csproj", "CoreBanking.Api/"]
COPY ["CoreBanking.Application/CoreBanking.Application.csproj", "CoreBanking.Application/"]
COPY ["CoreBanking.Domain/CoreBanking.Domain.csproj", "CoreBanking.Domain/"]
COPY ["CoreBanking.Infrastructure/CoreBanking.Infrastructure.csproj", "CoreBanking.Infrastructure/"]
RUN dotnet restore "./CoreBanking.Api/CoreBanking.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/CoreBanking.Api"
RUN dotnet build "./CoreBanking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ── Publish stage ──
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CoreBanking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ── Final production image ──
FROM base AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Run as non-root user
USER $APP_UID

COPY --from=publish /app/publish .

# Health check: verify the API is responding
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "CoreBanking.Api.dll"]
