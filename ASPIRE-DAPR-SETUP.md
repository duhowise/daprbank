# Dapr Sidecar Configuration for ASP.NET Aspire

This document describes how to run the BankService with Dapr as a sidecar using ASP.NET Aspire orchestration.

## Running with Dapr Sidecar

### Prerequisites
1. Dapr CLI installed (`dapr init`)
2. ASP.NET Aspire workload installed
3. Redis running (handled by Aspire)

### Configuration

The system is now configured to run with Dapr as a sidecar instead of embedded. The key changes are:

1. **AppHost Configuration**: The Aspire AppHost orchestrates both the BankService and Dapr sidecar
2. **BankService**: Continues to use Dapr APIs but no longer embeds Dapr runtime
3. **Components**: Dapr components (Redis state store and pub/sub) are configured externally

### Running the Application

```bash
# Install Aspire workload (if not already installed)
dotnet workload install aspire

# Run the AppHost (this will start Redis, BankService, and Dapr sidecar)
cd src/DaprBank.AppHost
dotnet run
```

This will:
- Start Redis container for state store and pub/sub
- Start the BankService
- Start Dapr sidecar attached to BankService
- Provide Aspire dashboard for monitoring and diagnostics

### Manual Dapr Sidecar (Alternative)

If running without Aspire orchestration:

```bash
# Terminal 1: Start Redis
docker run -d -p 6379:6379 redis:latest

# Terminal 2: Start BankService with Dapr sidecar
cd src/BankService
dapr run --app-id bankservice --app-port 5000 --components-path ../../components -- dotnet run

# Terminal 3: Access the service
curl http://localhost:3500/v1.0/invoke/bankservice/method/api/accounts/123/create -X POST -H "Content-Type: application/json" -d '{"AccountName": "Test Account"}'
```

## Key Benefits

1. **Service Composition**: Aspire provides unified orchestration
2. **Observability**: Built-in dashboards and diagnostics
3. **Sidecar Architecture**: Dapr runs as separate process, not embedded
4. **Development Experience**: Simplified local development with one command
5. **Production Ready**: Follows cloud-native patterns with sidecar architecture