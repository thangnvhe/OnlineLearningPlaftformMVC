# OnlineLearning

Project: OnlineLearning — an ASP.NET Core web application for online courses, mentorship, realtime chat, payments, and AI-assisted features.

## Table of contents
- Overview
- Features
- Architecture
- Quickstart (local)
- Configuration & environment
- Database & migrations
- Testing
- Folder map
- Deployment notes
- Contributing
- Contact

## Overview
OnlineLearning is a modular learning platform built with ASP.NET Core. The application supports:
- Course management (create, list, enroll)
- Role-based users (Admin / Mentor / Mentee)
- Realtime chat and notifications using SignalR
- Payments via Momo (integration present)
- AI training data and OpenAI integration

## Features
- User authentication (Google OAuth supported)
- Course catalog, lessons, and enrollments
- Messaging and chat hubs (`Hubs/`)
- Email notifications via SMTP
- Video / media helpers and YouTube API integration
- Data export/import via EPPlus

## Architecture
- Framework: .NET 8 (target: `net8.0`)
- Data access: Entity Framework Core with SQL Server provider
- Realtime: SignalR hubs for chat and notifications
- Background tasks: (see `Services/` folder for implementations)
- UI: Razor views under `Views/` and area-specific views in `Areas/`

Key packages (see `OnlineLearning.csproj`): `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.AspNetCore.Authentication.Google`, `OpenAI`, `EPPlus`, `RestSharp`, `Microsoft.AspNetCore.SignalR.Client`.

## Quickstart (local)
1. Requirements
   - .NET SDK 8.x installed (project target: `net8.0`)
   - SQL Server (local or remote)

2. Restore and build

```powershell
dotnet restore
dotnet build
```

3. Configure connection string and secrets (see next section)

4. Apply migrations and create DB

```powershell
# install tools if needed
dotnet tool install --global dotnet-ef
dotnet ef database update --project OnlineLearning.csproj
```

5. Run the app

```powershell
dotnet run --project OnlineLearning.csproj
```

The default URLs are configured in `Program.cs` — open the console output to see the listening address.

## Configuration & environment
- Primary config file: [appsettings.json](appsettings.json)
- Development overrides: `appsettings.Development.json`
- Recommended secret storage: `dotnet user-secrets` or environment variables.

Important configuration keys (example paths in `appsettings.json`):
- `ConnectionStrings:OLS` — SQL Server connection string
- `Smtp` — Host, Port, Username, Password for email
- `Authentication:Google:ClientId` and `ClientSecret` — Google OAuth
- `OpenAI:ApiKey` — OpenAI API key
- `MomoAPI` — payment integration (PartnerCode, SecretKey, AccessKey, ReturnUrl, NotifyUrl)
- `GoogleSheets` — credentials path and spreadsheet id for sheets integration

Example: set a user secret for OpenAI key

```powershell
cd d:\Ref_Project_Space\OnlineSystemLearning\OnlineLearning
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "<your-key>"
```

Or set environment variables (Windows PowerShell):

```powershell
$env:OpenAI__ApiKey = "<your-key>"
```

## Database & migrations
- DbContext: `Data/OnlLearnDBContext.cs`
- Migrations are stored in `Migrations/`.
- Add a new migration:

```powershell
dotnet ef migrations add <Name> --project OnlineLearning.csproj
```

- Apply migrations:

```powershell
dotnet ef database update --project OnlineLearning.csproj
```

## Testing
- Unit and integration tests (xUnit) live in the `Tests/` folder. Run:

```powershell
dotnet test
```

Use `Moq` for mocking and `Microsoft.NET.Test.Sdk` test runner already referenced in the project.

## Folder map (key folders)
- `Controllers/` — MVC controllers
- `Views/`, `Areas/` — Razor views and area-specific pages
- `Data/` — `OnlLearnDBContext.cs`, data converters, seeders
- `Migrations/` — EF Core migrations
- `Hubs/` — SignalR hubs
- `Configurations/` — helper classes to configure services (DI, DbContext, auth, sessions)
- `Services/` — business logic services and background tasks
- `Repositories/` — data access abstractions
- `wwwroot/` — static assets (css, js, images)

## Deployment notes
- Do not store secrets in `appsettings.json` for production. Use environment variables or a secrets manager.
- Configure HTTPS and reverse proxy (IIS, Nginx, or Azure App Service).
- If deploying to containers, add a `Dockerfile` and consider a `docker-compose` with a SQL Server container for local testing.

## CI / CD
- Add a GitHub Actions or Azure Pipelines workflow that runs `dotnet restore`, `dotnet build`, `dotnet test`, and optionally `dotnet ef database update` for integration tests against a disposable DB.

## Contributing
- Use feature branches and open PRs with clear descriptions.
- Ensure tests pass locally before creating PRs.
- Keep secrets out of commits.

## License & Authors
- Add your license file (e.g., `LICENSE`) if you wish to publish this repo.
- Add authors/maintainers and contact info here.

---

If you want, I can next:
- add a `CONTRIBUTING.md` template,
- create a `docker-compose.yml` for local SQL Server and the app,
- or open a commit/PR with these README changes.
