# ── Base runtime image ──
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

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

COPY --from=publish /app/publish .

# Default port — Render overrides via PORT env var
ENV PORT=8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CoreBanking.Api.dll"]
