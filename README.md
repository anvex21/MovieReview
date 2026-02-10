# MovieReview

## Description
MovieReview is an ASP.NET Core–based movie review application that combines a RESTful Web API with a built-in frontend.
The backend handles secure user authentication, movie and review CRUD operations, and advanced querying (filtering, sorting, pagination), while the frontend (served from wwwroot) lets users browse movies and manage their reviews directly in the browser.

---

## Features
**User Authentication & Authorization:**  
- Uses ASP.NET Core Identity and JWT tokens for secure access to API endpoints.  
- All operations except `Register` and `Login` require a Bearer token.

**Domain Functionality:**  
- **MoviesController:** Create, read, update, delete movies; filter, sort, paginate; get top-rated movies or movies by year.  
- **ReviewsController:** Add, read, update, delete reviews; get reviews by movie or user.

**Architecture & Tools:**  
- Entity Framework Core for ORM and database migrations.  
- Repository Pattern to abstract data access logic.  
- AutoMapper for mapping between entities and DTOs.  
- Swagger (Swashbuckle) for auto-generated API documentation.

---

## Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- MySQL 8.0+ (or MariaDB / compatible server)  
- IDE: Visual Studio 2022, Rider, or VS Code with C# extension

---

## Getting Started

1. **Clone the repo:**
```bash
git clone https://github.com/anvex21/MovieReview.git
cd MovieReview
```

2. **Update connection string**
Edit appsettings.json to set your MySQL connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=MovieReview;User=root;Password=yourpassword;"
}
```

3. **Apply migrations and update database**  
   The project is configured for MySQL. Old SQL Server migrations were removed. Create and apply the initial migration:
   ```bash
   cd MovieReview
   dotnet ef migrations add InitialMySql
   dotnet ef database update
   ```
   Ensure MySQL is running and the connection string in `appsettings.json` is correct. If you use a different MySQL server version, change the `MySqlServerVersion` in `Program.cs` (e.g. `new Version(8, 0, 21)` to match your server).



4. **Running the application**

    Using Visual Studio / Rider: Press F5 or click the Start button.

    Using the .NET CLI: Run the following command from the MovieReview directory:
    ```bash
    dotnet run --project MovieReview/MovieReview.csproj --launch-profile https
    ```

    Available endpoints:
    - Frontend UI: https://localhost:7178 (Opens by default)
    - API Documentation (Swagger): https://localhost:7178/swagger

    Note: The CSS is pre-built, but if you modify the styles, you can rebuild them using npm run build:css inside the MovieReview folder.

---

## Authentication

1. **Register a new user**
```bash
POST /api/Auth/Register
Content-Type: application/json

{
  "username": "testuser",
  "password": "Test123!",
  "email": "test@example.com"
}
```

2. **Login**
```bash
POST /api/Auth/Login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Test123!"
}
```

The response of the login request returns a **JWT token.** Include this token as Authorization: Bearer <token> for all protected endpoints.

---

## Testing
The solution includes a dedicated test project (`MovieReview.Tests`) to ensure API reliability and core logic integrity.

### **Running Tests**
* **Using Visual Studio / Rider:** 
    1. Open the **Unit Tests** window.
    2. Click **Run All** (or press `Ctrl+U, L`).
* **Using the .NET CLI:**
    Run the following command from the root directory:
    ```bash
    dotnet test
    ```
---

## Optional: IMDb / OMDb integration

The app can display **IMDb ratings** for movies by querying the public OMDb API.  
This integration is **optional** and is disabled if no API key is configured.

- **1. Get an OMDb API key (optional)**
  - Sign up at `https://www.omdbapi.com/` and obtain a free API key.

- **2. Configure the key using .NET user secrets (recommended for local dev)**

  From the repository root, run:

  ```bash
  dotnet user-secrets init --project MovieReview/MovieReview.csproj
  dotnet user-secrets set "Omdb:ApiKey" "your-omdb-api-key" --project MovieReview/MovieReview.csproj
  ```

  This stores the key **outside source control**, so it is not committed to GitHub.

- **3. Run the app as usual**

  ```bash
  dotnet run --project MovieReview/MovieReview.csproj --launch-profile https
  ```

If the key is not set or OMDb does not find a matching movie, the UI will simply show **`N/A/10`** for the IMDb rating.

## Notes
**Without a valid Bearer token, you can only access Register and Login.**

**CRUD operations for movies and reviews require authentication.**

**Filtering, sorting, and pagination work only on the GetAllMoviesWithQuery endpoint.**
