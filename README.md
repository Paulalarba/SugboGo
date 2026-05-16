# SugboGo

SugboGo is a comprehensive ASP.NET Core MVC travel platform designed to help users discover and experience Cebu like a local. Beyond a simple landing page, SugboGo offers personalized travel recommendations, booking management, and a dedicated administrator portal.

## Key Features

- **Personalized Exploration:** Discover tours, hiking trails, gastronomy experiences, and trending spots tailored to your preferences.
- **Travel Preferences Survey:** A dynamic survey that captures your travel style to provide curated recommendations.
- **Booking System:** Seamlessly plan and manage your adventures across Cebu.
- **User Dashboard:** A personalized hub to view saved gems, upcoming bookings, and travel history.
- **Admin Portal:** Comprehensive management interface for administrators to monitor platform activity and manage travel data.
- **Flexible Data Architecture:** Supports multiple backends (Local JSON, PostgreSQL, and Supabase) through a factory-based service pattern.
- **Secure Authentication:** Robust cookie-based authentication with role-based access control (Admin/User).

## Tech Stack

- **Framework:** ASP.NET Core MVC 10.0
- **Database:** PostgreSQL via Npgsql & Entity Framework Core
- **Cloud Integration:** Optional Supabase backend support
- **Authentication:** Custom Cookie-based Auth with PBKDF2 password hashing
- **Frontend:** Razor Views, Vanilla CSS, and JavaScript
- **Configuration:** `dotenv.net` for environment variable management

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) (Optional, but recommended for full features)

## Getting Started

### 1. Configuration

The application uses `appsettings.json` and a `.env` file for configuration.

- Create a `.env` file in the root directory (refer to `.env.example` if available, or add your `DefaultConnection`).
- Configure your PostgreSQL connection string in `appsettings.json` or `.env`:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SugboGo;Username=postgres;Password=yourpassword"
  }
  ```

### 2. Database Setup

Migrations are automatically applied on startup if a valid connection string is provided. The system also seeds initial travel spot data automatically.

### 3. Running the App

From the project root:

```powershell
dotnet restore
dotnet run
```

Access the application at:
- `http://localhost:5115`
- `https://localhost:7225`

## Project Structure

- `Controllers/`: Application logic for Explore, Booking, Admin, and Account management.
- `Models/`: Data structures and ViewModels.
- `Services/`: Business logic layer, including Auth, Travel, Booking, and Admin services.
- `Data/`: EF Core DbContext, Migrations, and Seeding logic.
- `Views/`: UI templates using Razor syntax.
- `App_Data/`: Local storage for JSON data files and Data Protection keys.
- `wwwroot/`: Static assets (CSS, Images, JS, Libs).

## Notes

- **Default Route:** The app launches to the `Home/Index` landing page.
- **Storage Strategy:** The application defaults to a specific storage backend (JSON or Postgres) based on configuration. This is managed by service factories in `Program.cs`.
- **Admin Access:** Administrator roles are determined by email addresses configured in `appsettings.json` under `Authentication:AdminEmails`.
