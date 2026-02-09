# MovieReview

## Description
MovieReview is a fully-featured backend ASP.NET Core Web API designed to manage movies and reviews.  
It supports secure user authentication, movie and review CRUD operations, and filtering, sorting, and pagination for movies.

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

4. **Run the application with F5**

5. **Frontend (optional)**  
   The API and the web UI are served together. When you run the project, the app (login, movies, reviews) is available at the same URL as the API, e.g. **https://localhost:7178** or **http://localhost:5240** — open that URL in the browser for the frontend; Swagger remains at `/swagger`.  
   To rebuild the frontend CSS (Tailwind), from the `MovieReview` project folder run:
   ```bash
   cd MovieReview
   npm install
   npm run build:css
   ```
   Use `npm run watch:css` while changing styles or markup.

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

## Notes
**Without a valid Bearer token, you can only access Register and Login.**

**CRUD operations for movies and reviews require authentication.**

**Filtering, sorting, and pagination work only on the GetAllMoviesWithQuery endpoint.**
