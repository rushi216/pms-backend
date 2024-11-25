# PMS Backend (Performance Management System)

A .NET Core backend service for managing employee performance reviews and user synchronization with Microsoft Graph API.

## Prerequisites

- .NET 7.0 or later
- PostgreSQL
- Azure AD Application registration (for Microsoft Graph API)

## Configuration

1. Update `appsettings.Development.json` with your values:
   - Azure AD credentials
   - Database connection string
   - Business license SKUs

## Running the Application

1. Install dependencies:
   ```bash
   dotnet restore
   ```

2. Update database:
   ```bash
   dotnet ef database update
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at `http://localhost:5200`

## Features

- User synchronization with Microsoft Graph API
- Performance review management
- Manager-reportee relationship tracking
- Azure AD authentication
