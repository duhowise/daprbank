# Dapr Bank - Event-Sourced Banking System

A simple event-sourced banking system built with .NET and Dapr Actors, featuring account management, transactions, and event publishing via PubSub.

## Features

✅ **Actor System**
- `IAccountActor` interface with account operations
- `AccountActor` implementation with state management and event publishing

✅ **Domain Models**
- `AccountState` class for actor persistence
- Event records: `AccountCreated`, `MoneyDeposited`, `MoneyWithdrawn`

✅ **REST API**
- Account creation, deposits, withdrawals, and balance queries
- Comprehensive error handling and validation

✅ **Dapr Integration**
- Actor state management using Dapr state store
- Event publishing via Dapr PubSub

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/accounts/{id}/create` | Create a new account |
| POST | `/api/accounts/{id}/deposit` | Deposit money to account |
| POST | `/api/accounts/{id}/withdraw` | Withdraw money from account |
| GET | `/api/accounts/{id}/balance` | Get account balance |

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
- [Docker](https://docs.docker.com/get-docker/) (for local development with Dapr)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/duhowise/daprbank.git
   cd daprbank
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run tests:
   ```bash
   dotnet test
   ```

### Running with Dapr

1. Initialize Dapr (if not already done):
   ```bash
   dapr init
   ```

2. Run the application with Dapr:
   ```bash
   dapr run --app-id bankservice --app-port 5294 --dapr-http-port 3500 --resources-path ./components -- dotnet run --project src/BankService
   ```

### API Usage Examples

1. **Create an account:**
   ```bash
   curl -X POST http://localhost:5294/api/accounts/acc123/create \
     -H "Content-Type: application/json" \
     -d '{"accountName": "John Doe Account"}'
   ```

2. **Deposit money:**
   ```bash
   curl -X POST http://localhost:5294/api/accounts/acc123/deposit \
     -H "Content-Type: application/json" \
     -d '{"amount": 100.50}'
   ```

3. **Withdraw money:**
   ```bash
   curl -X POST http://localhost:5294/api/accounts/acc123/withdraw \
     -H "Content-Type: application/json" \
     -d '{"amount": 25.00}'
   ```

4. **Check balance:**
   ```bash
   curl http://localhost:5294/api/accounts/acc123/balance
   ```

## Project Structure

```
├── src/
│   └── BankService/           # Main service project
│       ├── Actors/            # Actor interfaces and implementations
│       ├── Models/            # Domain models and events
│       ├── Controllers/       # REST API controllers
│       └── Program.cs         # Application startup and Dapr configuration
├── Tests/
│   └── BankService.Tests/     # Unit tests
└── DaprBank.sln              # Solution file
```

## Architecture

- **Actors**: Dapr Actors provide a stateful, single-threaded model for account management
- **State Management**: Account state is persisted using Dapr state store
- **Event Publishing**: Domain events are published via Dapr PubSub for event sourcing
- **REST API**: Standard HTTP endpoints for account operations

## Events Published

- `AccountCreated`: When a new account is created
- `MoneyDeposited`: When money is deposited to an account  
- `MoneyWithdrawn`: When money is withdrawn from an account

Events are published to the `account-events` topic using the default Dapr PubSub component.

## Development

The application is built with:
- **.NET 8.0** - Runtime and framework
- **Dapr Actors** - For stateful actor model
- **Dapr PubSub** - For event publishing
- **ASP.NET Core** - For REST API
- **Swagger/OpenAPI** - For API documentation

## Testing

The project includes unit tests for:
- Domain models and events
- Account state management
- Event record equality and properties

Run tests with: `dotnet test`

## Dapr Components

The application expects standard Dapr components for:
- **State Store**: For actor state persistence (default: Redis)
- **PubSub**: For event publishing (default: Redis Streams)

Component configurations can be added to a `components/` directory for custom setups. 
