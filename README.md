# Dapr Bank - Event-Sourced Banking System

A simple event-sourced banking system built with .NET and Dapr Actors, featuring account management, transactions, and event publishing via PubSub. **Now enhanced with ASP.NET Aspire orchestration and Dapr sidecar integration.**

## 🌟 New: ASP.NET Aspire + Dapr Sidecar Architecture

This project has been migrated to use **ASP.NET Aspire** for service composition, dashboarding, and diagnostics, while running **Dapr as a sidecar** instead of embedded in the app runtime.

### Key Benefits
- 🚀 **Unified Orchestration**: Aspire manages Redis, BankService, and Dapr sidecar
- 📊 **Built-in Observability**: Aspire dashboard for monitoring and diagnostics  
- 🔄 **Sidecar Architecture**: Dapr runs as separate process, following cloud-native patterns
- 🛠️ **Simplified Development**: One command to start entire system
- 📈 **Production Ready**: Follows microservices best practices

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
- **Sidecar mode** for production-ready architecture

✅ **ASP.NET Aspire**
- Service orchestration and composition
- Built-in dashboards and diagnostics
- Redis container management

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
- [Docker](https://docs.docker.com/get-docker/) (for Redis and local development)
- **[ASP.NET Aspire workload](https://learn.microsoft.com/aspire/fundamentals/setup-tooling)** (for full orchestration)

### Quick Start (Aspire + Dapr Sidecar)

1. **Install Aspire workload:**
   ```bash
   dotnet workload install aspire
   ```

2. **Clone and build:**
   ```bash
   git clone https://github.com/duhowise/daprbank.git
   cd daprbank
   dotnet build
   ```

3. **Run with Aspire orchestration:**
   ```bash
   cd src/DaprBank.AppHost
   dotnet run
   ```
   This starts Redis, BankService, and provides the Aspire dashboard at `http://localhost:15888`

### Alternative: Manual Dapr Sidecar

If you prefer to run without Aspire:

1. **Initialize Dapr:**
   ```bash
   dapr init
   ```

2. **Run with provided script:**
   ```bash
   # Linux/macOS
   ./run-with-dapr-sidecar.sh
   
   # Windows PowerShell
   .\run-with-dapr-sidecar.ps1
   ```

3. **Or run manually:**
   ```bash
   # Start Redis
   docker run -d -p 6379:6379 redis:latest
   
   # Start with Dapr sidecar
   cd src/BankService
   dapr run --app-id bankservice --app-port 5000 --dapr-http-port 3500 --components-path ../../components -- dotnet run
   ```

### Running Tests

```bash
dotnet test
```

### API Usage Examples

**Access the API:**
- **Aspire**: Through Dapr sidecar at `http://localhost:3500/v1.0/invoke/bankservice/method/api/accounts/`
- **Direct**: BankService at `http://localhost:5000/api/accounts/` (when running standalone)

1. **Create an account:**
   ```bash
   # Via Dapr sidecar (recommended)
   curl -X POST http://localhost:3500/v1.0/invoke/bankservice/method/api/accounts/acc123/create \
     -H "Content-Type: application/json" \
     -d '{"accountName": "John Doe Account"}'
   
   # Direct to service (alternative)
   curl -X POST http://localhost:5000/api/accounts/acc123/create \
     -H "Content-Type: application/json" \
     -d '{"accountName": "John Doe Account"}'
   ```

2. **Deposit money:**
   ```bash
   curl -X POST http://localhost:3500/v1.0/invoke/bankservice/method/api/accounts/acc123/deposit \
     -H "Content-Type: application/json" \
     -d '{"amount": 100.50}'
   ```

3. **Withdraw money:**
   ```bash
   curl -X POST http://localhost:3500/v1.0/invoke/bankservice/method/api/accounts/acc123/withdraw \
     -H "Content-Type: application/json" \
     -d '{"amount": 25.00}'
   ```

4. **Check balance:**
   ```bash
   curl http://localhost:3500/v1.0/invoke/bankservice/method/api/accounts/acc123/balance
   ```

## Project Structure

```
├── src/
│   ├── BankService/            # Main service project
│   │   ├── Actors/             # Actor interfaces and implementations
│   │   ├── Models/             # Domain models and events
│   │   ├── Controllers/        # REST API controllers
│   │   └── Program.cs          # Application startup and Dapr configuration
│   └── DaprBank.AppHost/       # 🆕 Aspire orchestration project
│       ├── DaprBank.AppHost.csproj
│       └── Program.cs          # Service composition and configuration
├── Tests/
│   └── BankService.Tests/      # Unit tests
├── components/                 # Dapr component configurations
│   ├── statestore.yaml        # Redis state store
│   └── pubsub.yaml            # Redis pub/sub
├── dapr-config.yaml           # 🆕 Dapr configuration for sidecar
├── run-with-dapr-sidecar.sh   # 🆕 Manual sidecar launch script
├── run-with-dapr-sidecar.ps1  # 🆕 PowerShell version
├── ASPIRE-DAPR-SETUP.md       # 🆕 Detailed setup documentation
└── DaprBank.sln               # Solution file
```

## Architecture

### Current: ASP.NET Aspire + Dapr Sidecar

- **🏗️ Aspire AppHost**: Orchestrates the entire application stack
- **🔄 Dapr Sidecar**: Runs as separate process, not embedded in application
- **🗄️ Actors**: Dapr Actors provide stateful, single-threaded model for account management
- **💾 State Management**: Account state persisted using Redis via Dapr state store
- **📢 Event Publishing**: Domain events published via Dapr PubSub for event sourcing
- **🌐 REST API**: Standard HTTP endpoints accessible via Dapr service invocation
- **📊 Observability**: Aspire dashboard provides monitoring and diagnostics

### Migration Benefits

✅ **Before (Embedded Dapr)**:
- Dapr runtime embedded in application
- Manual service coordination
- Limited observability

✅ **After (Aspire + Sidecar)**:
- Dapr runs as separate sidecar process
- Unified service orchestration with Aspire
- Built-in dashboards and monitoring
- Production-ready microservices architecture

## Events Published

- `AccountCreated`: When a new account is created
- `MoneyDeposited`: When money is deposited to an account  
- `MoneyWithdrawn`: When money is withdrawn from an account

Events are published to the `account-events` topic using the default Dapr PubSub component.

## Development

The application is built with:
- **.NET 8.0** - Runtime and framework
- **ASP.NET Aspire** - Service composition and orchestration
- **Dapr Actors** - For stateful actor model (sidecar mode)
- **Dapr PubSub** - For event publishing
- **ASP.NET Core** - For REST API
- **Redis** - State store and pub/sub (managed by Aspire)
- **Swagger/OpenAPI** - For API documentation

## Accessing Services

| Service | URL | Description |
|---------|-----|-------------|
| Aspire Dashboard | `http://localhost:15888` | Service monitoring and logs |
| BankService (Direct) | `http://localhost:5000` | Direct API access |
| BankService (via Dapr) | `http://localhost:3500/v1.0/invoke/bankservice/method/` | Dapr service invocation |
| Dapr Dashboard | `http://localhost:8080` | Dapr-specific monitoring |

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
