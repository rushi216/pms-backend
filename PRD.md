# Performance Review Tracker - Backend

## Product Description
The Performance Review Tracker backend will manage user data, reviews, and role-specific operations. It will handle authentication, data storage, and business logic for performance review tracking.

## Objectives
- Provide secure API endpoints for frontend interaction.
- Store and manage user, manager, and review data.
- Integrate with Microsoft Graph API for user synchronization.
- Enforce authentication and role-based access control.

## Tech Stack
- **Framework**: ASP.NET Core 8
- **ORM**: Entity Framework
- **Database**: PostgreSQL
- **Authentication**: Bearer token verification with Azure Entra ID.
- **Integration**: Microsoft Graph API for user data sync.

## Features

### Authentication
- Validate bearer tokens received from the frontend.
- Extract user ID and email from tokens for role and data validation.

### Database Design
- **Tables**:
  - **User**: `Id`, `Email`, `MicrosoftID`, `Name`.
  - **User-Manager Mapping**: `Id`, `UserId`, `ManagerId`, `AssignmentTimestamp`, `IsDeleted`.
  - **Review**: `Id`, `ForUserId`, `FromUserId`, `Year`, `Quarter`, `Q1`, `A1`, `Q2`, `A2`, `Q3`, `A3`.

### API Endpoints
1. **Authentication**:
   - Verify bearer tokens.
   - Provide user details for role validation.

2. **User Management**:
   - Fetch all users from the database.
   - Assign a manager to a user.
   - Fetch assigned employees for a manager.

3. **Reviews**:
   - Fetch reviews based on filters (year, quarter, user).
   - Update reviews with answers.

4. **Synchronization**:
   - Sync all users from Microsoft Graph API and update the database.
   - Resync functionality to handle updates or new users.

### Synchronization with Microsoft Graph API
- Periodically sync all organizational users.
- Store user data in the `User` table, updating existing records where applicable.

### Error Handling
- Return appropriate error codes and messages for:
  - Unauthorized access (401).
  - Resource not found (404).
  - Validation errors (400).
  - Server errors (500).

### Security
- Validate incoming tokens to prevent unauthorized access.
- Use role-based checks to restrict admin operations.