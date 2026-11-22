# TSMA Meal

A web-based meal reservation system built using **Vertical Slice Architecture**, developed to replace TSMA's previous meal reservation application.

## Overview

TSMA Meal provides employees with a centralized platform to authenticate via SMS OTP, view weekly meal menus, and manage their meal reservations.

The application was built on top of an existing legacy database with a non-standard schema, structural limitations, and complex business logic embedded in its existing implementation. Adapting the application to these constraints required handling legacy behaviors, including custom scheduling rules such as alternating-week changes.

## Architecture

The application follows **Vertical Slice Architecture**, organizing the codebase around business features rather than technical layers.

Each feature encapsulates its related application logic, keeping concerns isolated and making the codebase easier to maintain and extend.

The project also uses **CQRS** with **MediatR** to handle application requests.

## Features

- SMS OTP Authentication
- Weekly Meal Menu
- Meal Reservations
- Reservation Cancellation
- Persian RTL Interface

## Tech Stack

| Technology | Purpose |
|---|---|
| .NET 8 | Backend Framework |
| ASP.NET Core | Web Application |
| Blazor Web App (Interactive Server) | UI Framework |
| Entity Framework Core 8 | ORM |
| SQL Server | Database |
| MediatR | CQRS |
| AutoMapper | Object Mapping |
| xUnit | Testing |
| Moq | Mocking |

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (not needed in [demo mode](#demo-mode)) 
- IPPanel SMS Service Account (not needed in [demo mode](#demo-mode))

### Installation

Clone the repository:

```bash
git clone https://github.com/adnvnnn/TSMA_Meal.git
cd TSMA_Meal
```

Restore dependencies:

```bash
dotnet restore
```

### Configuration

Copy the example configuration file:

```bash
cp Web/appsettings.Development.Example.json Web/appsettings.Development.json
```

On Windows PowerShell:

```powershell
Copy-Item Web/appsettings.Development.Example.json Web/appsettings.Development.json
```

Update the configuration with your environment-specific values.

| Configuration Key | Description |
|---|---|
| `ConnectionStrings:Default` | SQL Server connection string |
| `ApiConfig:BaseUrl` | API base URL |
| `SmsService:Username` | SMS service username |
| `SmsService:Password` | SMS service password |
| `SmsService:Sender` | SMS sender number |
| `SmsService:VerificationPatternCode` | SMS verification pattern code |
| `SmsService:Url` | SMS service endpoint |
| `SecuritySettings:LoginPath` | Route used for the login page |
| `SecuritySettings:LogoutPath` | Route used to sign the user out |
| `SecuritySettings:LoginDays` | Number of days a login stays valid |
| `SecuritySettings:OtpExpiryMinutes` | How long a sent OTP code remains valid |
| `SecuritySettings:MaxOtpAttemptsPerDay` | Daily limit of OTP requests per user |
| `SecuritySettings:MaxCheckOtpAttemptsPerSend` | Verification attempts allowed per sent code |
| `SecuritySettings:BanDurationHours` | Lockout duration after exceeding the limits |

### Demo Mode (no database, no SMS account)

The example configuration ships with `DemoMode` enabled. In demo mode the application uses an in-memory database seeded with one employee and a week of meals (Saturday to Thursday), and the OTP code is printed to the console instead of being sent through IPPanel.

Start the app in demo mode:

```bash
dotnet run --project Web
```

To enable it without editing the file, pass the setting on the command line:

```bash
DemoMode=true dotnet run --project Web
```

On Windows PowerShell:

```powershell
$env:DemoMode="true"; dotnet run --project Web
```

Sign in with the seeded demo employee:

| Field | Value |
|---|---|
| Name | کاربر نمایشی |
| Personnel code | `1001` |
| Phone number | `09120000000` |
| OTP code | printed in the console after pressing send |

What the demo database contains:

- one active employee with the details above, a 20% discount and food reservation access
- meals for Saturday to Thursday (Friday is a weekend day), two dishes per day, each with its own photo under `Web/wwwroot/images/meals/`
- all meals belong to "رستوران نمایشی" and cost `150000`

When you press send on the login page, the console prints a line like:

```
DEMO OTP for 09120000000: 123456
```

Enter that code to finish signing in. The in-memory data is not persisted — restarting the app clears any reservations. Set `DemoMode` to `false` to use SQL Server and IPPanel again.

### Run the Application

Build the project:

```bash
dotnet build
```

Run the application:

```bash
dotnet run --project Web
```

Default development URLs:

- https://localhost:7096
- http://localhost:5209

## Database Notes

This project uses an existing legacy database inherited from the previous application. Both its database schema and parts of the existing application logic contain non-standard design decisions, structural limitations, and tightly coupled business rules.

The application had to accommodate these constraints, including complex scheduling behaviors such as alternating-week changes, rather than relying on a clean or conventional data model.

No Entity Framework migrations are included. The application expects the existing database structure to be available before running.

Database-related resources are located in Web/Database.

## Testing

Run the test suite:

```bash
dotnet test
```

Tests are implemented using **xUnit** and **Moq**.

## Project Structure

```text
TSMA_Meal/
└── Web/
    ├── Components/
    ├── Database/
    ├── Features/
    ├── Setup/
    ├── Shared/
    ├── Tools/
    └── ...
```

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
