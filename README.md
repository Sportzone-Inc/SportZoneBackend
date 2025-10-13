# SportZone Backend

## Overview
SportZone Backend is a .NET 9.0 Web API project for managing sports-related functionalities.

## Technology Stack
- .NET 9.0
- ASP.NET Core Web API
- NUnit for unit testing

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application
```bash
dotnet restore
dotnet build
dotnet run --project SportZone/SportZone.csproj
```

## Development Workflow
1. Create a new branch for each ticket
2. Write 3 unit tests with NUnit first (TDD approach)
3. Implement functionality
4. Create pull request to master

## Testing
Unit tests are written using NUnit framework.

```bash
dotnet test
```

## Project Structure
- `SportZone/` - Main API project
- `SportZone.Tests/` - Unit tests (to be created)

---
*This is a test PR to validate the workflow*
