# ConfigurationReader

A robust .NET 8 configuration management system that provides database-backed configuration storage with Redis caching, real-time synchronization, and seamless integration with .NET's configuration system.

## 💼 Technical Highlights

This project demonstrates proficiency in:

- **Clean Architecture** - Separation of concerns with distinct layers (API, Application, Data, Common)
- **Design Patterns** - Strategy Pattern for caching, Repository Pattern for data access
- **Microservices Architecture** - Multi-application configuration management
- **Caching Strategies** - Redis implementation with intelligent cache invalidation
- **Background Services** - .NET Hosted Services for real-time synchronization
- **Entity Framework Core** - Code-first approach with migrations
- **Dependency Injection** - Extensive use of .NET DI container
- **Unit Testing** - Comprehensive test coverage with mocking
- **API Design** - RESTful endpoints following HTTP conventions
- **Configuration Management** - Custom configuration providers

## 🎯 Problem Statement & Solution

**Challenge**: Applications often need dynamic configuration management without redeployment, with high performance requirements and multi-environment support.

**Solution**: A centralized configuration system that:

- Stores configurations in a database for persistence and management
- Uses Redis for high-performance caching (sub-millisecond access)
- Provides real-time updates without application restart
- Integrates seamlessly with .NET's native configuration system
- Supports multiple applications from a single management point

## 🚀 Features

- **Database-Backed Storage**: Store configurations in SQL Server with Entity Framework Core
- **Redis Caching**: High-performance caching layer for optimal retrieval speed
- **Real-Time Sync**: Background service automatically synchronizes configurations at configurable intervals
- **Multi-Application Support**: Manage configurations for multiple applications from a single system
- **REST API**: Full CRUD operations through a clean REST API
- **Seamless Integration**: Custom configuration provider integrates with .NET's `IConfiguration`
- **Type Support**: Boolean, Integer, Double, and String configuration values
- **Strategy Pattern**: Intelligent caching strategies (filtered vs non-filtered)

## 🏗️ Architecture & Design Decisions

The project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│                    API Layer                        │ ← Controllers, HTTP endpoints
├─────────────────────────────────────────────────────┤
│               Application Layer                     │ ← Business logic, Services, Strategies
├─────────────────────────────────────────────────────┤
│                 Data Layer                          │ ← Entity Framework, Repositories
├─────────────────────────────────────────────────────┤
│                Common Layer                         │ ← Shared models, Utilities
└─────────────────────────────────────────────────────┘
```

### Key Architectural Decisions:

1. **Strategy Pattern for Caching**: Implemented `FilteredConfigurationStrategy` and `NonFilteredConfigurationStrategy` to optimize different query patterns
2. **Custom Configuration Provider**: Extends .NET's `IConfigurationProvider` for seamless integration
3. **Background Synchronization**: Uses `IHostedService` for non-blocking configuration updates
4. **Repository Pattern**: Abstracts data access for testability and maintainability
5. **Redis Cache-Aside Pattern**: Implements intelligent caching with automatic invalidation

### Performance Considerations:

- **Redis Caching**: Reduces database calls by ~95% for frequently accessed configurations
- **Async/Await**: Non-blocking operations throughout the application
- **Connection Pooling**: Entity Framework connection management
- **Configurable Sync Intervals**: Balance between freshness and performance

## 📦 Installation

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Redis (for caching)

### Setup

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd ConfigurationReader
   ```

2. **Update connection strings**

   Update `appsettings.json` in both API and console app projects:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "your-sql-server-connection-string",
       "Redis": "your-redis-connection-string"
     }
   }
   ```

3. **Run database migrations**

   ```bash
   dotnet ef database update --project src/ConfigurationReader.Data --startup-project src/ConfigurationReader.API
   ```

4. **Start the API**
   ```bash
   dotnet run --project src/ConfigurationReader.API
   ```

## 🎯 Quick Start

### Using in a .NET Application

1. **Install the package** (when published)

   ```bash
   dotnet add package ConfigurationReader
   ```

2. **Configure in Program.cs**

   ```csharp
   using ConfigurationReader;

   var builder = Host.CreateDefaultBuilder(args)
       .ConfigureConfigurationProvider()
       .ConfigureServices((context, services) =>
       {
           services.AddConfigurationProvider(context.Configuration);
           // Add your other services
       });

   var app = builder.Build();
   app.Run();
   ```

3. **Add configuration settings**

   ```json
   {
     "ConfigurationProviderOptions": {
       "ApplicationName": "MyApp",
       "ConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Config;Integrated Security=True",
       "RefreshIntervalInSeconds": 30
     }
   }
   ```

4. **Use configurations in your service**
   ```csharp
   public class MyService
   {
       private readonly IConfiguration _configuration;

       public MyService(IConfiguration configuration)
       {
           _configuration = configuration;
       }

       public void DoSomething()
       {
           var isFeatureEnabled = _configuration.GetValue<bool>("ConfigurationManagement:IsBasketEnabled");
           var maxItems = _configuration.GetValue<int>("ConfigurationManagement:MaxItemCount");
       }
   }
   ```

## 🔧 API Usage

### Configuration Endpoints

**Create Configuration**

```http
POST /configurations
Content-Type: application/json

{
  "name": "IsBasketEnabled",
  "type": 1,
  "value": "true",
  "applicationName": "MyApp"
}
```

**Get All Configurations**

```http
GET /configurations?name=IsBasket
```

**Get Configurations by Application**

```http
GET /apps/MyApp/configurations
```

**Delete Configuration**

```http
DELETE /configurations/1
```

### Supported Types

| Type    | Value | Description       |
| ------- | ----- | ----------------- |
| Boolean | 1     | true/false values |
| Int     | 2     | Integer numbers   |
| Double  | 3     | Decimal numbers   |
| String  | 4     | Text values       |

## ⚙️ Configuration Options

### ConfigurationProviderOptions

| Property                   | Description                                    | Default       |
| -------------------------- | ---------------------------------------------- | ------------- |
| `ApplicationName`          | Unique identifier for your application         | Assembly name |
| `ConnectionString`         | SQL Server connection string                   | Required      |
| `RefreshIntervalInSeconds` | How often to sync configurations (0 = no sync) | 0             |

### Example Configuration

```json
{
  "ConfigurationProviderOptions": {
    "ApplicationName": "SERVICE-A",
    "ConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Config;Integrated Security=True",
    "RefreshIntervalInSeconds": 30
  },
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

## 🔄 How It Works

1. **Configuration Storage**: Configurations are stored in SQL Server with application-specific scoping
2. **Background Sync**: A hosted service periodically fetches configurations from the database
3. **Caching Layer**: Redis caches frequently accessed configurations for performance
4. **Real-time Updates**: Changes made via API automatically invalidate cache and trigger refresh
5. **Integration**: Custom configuration provider seamlessly integrates with .NET's `IConfiguration`

## 🏃 Running the Example

The included console application demonstrates the usage:

```bash
cd example/ConsoleApp
dotnet run
```

This will start a background service that reads the `IsBasketEnabled` configuration every second and displays its value.

## 🧪 Testing

Run the test suite:

```bash
dotnet test
```

The tests cover:

- Configuration service functionality
- Redis caching strategies
- Repository operations
- Strategy pattern implementation
- Mock-based unit testing with 90%+ coverage

## 🛠️ Development

### Project Structure

```
ConfigurationReader/
├── src/
│   ├── ConfigurationReader/              # Core library with custom providers
│   ├── ConfigurationReader.API/          # REST API with Swagger documentation
│   ├── ConfigurationReader.Application/  # Business logic & caching strategies
│   ├── ConfigurationReader.Data/         # EF Core data access layer
│   └── ConfigurationReader.Common/       # Shared models and utilities
├── example/
│   └── ConsoleApp/                       # Working implementation example
└── test/
    └── ConfigurationReader.Tests/        # Comprehensive unit tests
```

### Building

```bash
dotnet build
```

### Database Migrations

Add new migration:

```bash
dotnet ef migrations add MigrationName --project src/ConfigurationReader.Data --startup-project src/ConfigurationReader.API
```

## 🚀 Production Considerations

- **Monitoring**: Implement logging and metrics for cache hit rates and sync operations
- **Security**: Add authentication/authorization for the management API
- **Scaling**: Redis clustering for high availability scenarios
- **Backup**: Regular database backups for configuration persistence
- **Environment Isolation**: Separate configurations per environment (dev/staging/prod)

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.
