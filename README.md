# TSMA Meal

A meal reservation web application for TSMA personnel. Users sign in with an SMS one-time
password, browse the weekly menu, reserve meals and cancel their reservations. The UI is in
Persian (RTL).

## Tech stack

- .NET 8 / ASP.NET Core Blazor Server (interactive server components)
- EF Core 8 with SQL Server
- MediatR (CQRS handlers) and AutoMapper
- Cookie authentication with OTP verification delivered through an SMS provider (ippanel SOAP API)
- xUnit + Moq for tests

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A SQL Server instance reachable from the machine running the app
- An SMS account (username, password, sender number and verification pattern code)

## Getting started

1. Clone the repository.

   ```bash
   git clone https://github.com/adnvnnn/TSMA_Meal.git
   cd TSMA_Meal
   ```

2. Create your local configuration from the example template.

   ```bash
   cp Web/appsettings.Development.Example.json Web/appsettings.Development.json
   ```

   Then edit `Web/appsettings.Development.json` and fill in:

   | Setting | Description |
   | --- | --- |
   | `ConnectionStrings:Default` | SQL Server connection string |
   | `ApiConfig:BaseUrl` | Base URL used by the app's HTTP client |
   | `SmsService:Username` / `Password` | SMS provider credentials |
   | `SmsService:Sender` | Sender number |
   | `SmsService:VerificationPatternCode` | Pattern code used for the OTP message |
   | `SmsService:Url` | SMS provider WSDL endpoint |

3. Restore and build.

   ```bash
   dotnet restore
   dotnet build
   ```

4. Run the app.

   ```bash
   dotnet run --project Web
   ```

   With the `https` launch profile the app listens on <https://localhost:7096> (and
   <http://localhost:5209>); see `Web/Properties/launchSettings.json`.

## Configuration

Configuration is loaded from `appsettings.json`, `appsettings.{Environment}.json` and
environment variables (see `Web/Setup/LogRegister.cs`), so production values can also be
supplied as environment variables instead of a file.

Local and production config files are **not** tracked by git:

- `Web/appsettings.Development.json` — local settings, created from `Web/appsettings.Development.Example.json`
- `Web/appsettings.Production.json` — deployment settings, created from `Web/appsettings.Production.Example.json`

Never commit real connection strings or SMS credentials.

The repository does not include EF Core migrations; point `ConnectionStrings:Default` at a
database that already has the expected schema (see `Web/Database`).

## Tests

The test suite lives next to the features it covers (`Web/Features/**/*Tests.cs`) and uses
xUnit with Moq.

```bash
dotnet test
```

## Project layout

```
Web/
  Components/        Blazor components and pages
  Database/          AppDbContext, entities and entity configurations
  Features/          Account (login, logout, OTP) and Meals (list, reserve, cancel) handlers + tests
  Setup/             Service registration and configuration binding
  Shared/            Repositories, services, middleware and exceptions
  Tools/             Utility helpers (Persian date/time, price formatting, encoding)
```

## License

[MIT](LICENSE)
