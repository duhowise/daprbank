#!/bin/bash

# Start Dapr Bank Service with Sidecar
# This script demonstrates how to run the BankService with Dapr sidecar manually

echo "Starting Dapr Bank Service with Sidecar..."

# Check if Dapr CLI is installed
if ! command -v dapr &> /dev/null; then
    echo "Error: Dapr CLI is not installed. Please install Dapr CLI first."
    echo "Visit: https://docs.dapr.io/getting-started/install-dapr-cli/"
    exit 1
fi

# Check if Redis is running (you may need to start Redis separately)
echo "Checking Redis connection..."
if ! command -v redis-cli &> /dev/null || ! redis-cli ping &> /dev/null; then
    echo "Warning: Redis is not running or redis-cli is not available."
    echo "Please start Redis with: docker run -d -p 6379:6379 redis:latest"
    echo "Continuing anyway..."
fi

# Navigate to BankService directory
cd "$(dirname "$0")/src/BankService" || exit

# Build the BankService
echo "Building BankService..."
dotnet build

# Run with Dapr sidecar
echo "Starting BankService with Dapr sidecar..."
echo "Dashboard will be available at: http://localhost:8080"
echo "BankService API will be available at: http://localhost:3500/v1.0/invoke/bankservice/method/"

dapr run \
    --app-id bankservice \
    --app-port 5000 \
    --dapr-http-port 3500 \
    --dapr-grpc-port 50001 \
    --components-path ../../components \
    --config ../../dapr-config.yaml \
    --log-level info \
    -- dotnet run --urls "http://localhost:5000"