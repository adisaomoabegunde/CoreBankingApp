#!/bin/bash
set -euo pipefail

# CoreBanking API - Docker Build Script
# Usage: ./scripts/docker-build.sh [--tag my-tag] [--push]

IMAGE_NAME="corebanking-api"
TAG="latest"
PUSH=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --tag) TAG="$2"; shift 2 ;;
        --push) PUSH=true; shift ;;
        *) echo "Unknown option: $1"; exit 1 ;;
    esac
done

FULL_TAG="${IMAGE_NAME}:${TAG}"

echo "=== Docker Build ==="
echo "Image: $FULL_TAG"
echo ""

# Build the Docker image
docker build -t "$FULL_TAG" .

echo "--- Image built: $FULL_TAG ---"

if [ "$PUSH" = true ]; then
    echo "--- Pushing image ---"
    docker push "$FULL_TAG"
    echo "--- Image pushed ---"
fi

echo "=== Docker build completed ==="
