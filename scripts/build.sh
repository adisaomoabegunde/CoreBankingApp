#!/bin/bash
set -euo pipefail

# CoreBanking API - Build & Test Script
# Usage: ./scripts/build.sh [--skip-tests] [--configuration Release|Debug]

CONFIGURATION="Release"
SKIP_TESTS=false
SOLUTION="Corebankingapp.sln"

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-tests) SKIP_TESTS=true; shift ;;
        --configuration) CONFIGURATION="$2"; shift 2 ;;
        *) echo "Unknown option: $1"; exit 1 ;;
    esac
done

echo "=== CoreBanking API Build ==="
echo "Configuration: $CONFIGURATION"
echo ""

# Step 1: Restore
echo "--- Restoring dependencies ---"
dotnet restore "$SOLUTION"

# Step 2: Build
echo "--- Building solution ---"
dotnet build "$SOLUTION" --configuration "$CONFIGURATION" --no-restore

# Step 3: Test
if [ "$SKIP_TESTS" = false ]; then
    echo "--- Running tests ---"
    dotnet test "$SOLUTION" \
        --configuration "$CONFIGURATION" \
        --no-build \
        --verbosity normal \
        --logger "trx;LogFileName=test-results.trx" \
        --collect:"XPlat Code Coverage"
    echo "--- Tests passed ---"
else
    echo "--- Tests skipped ---"
fi

echo ""
echo "=== Build completed successfully ==="
