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
- SQL Server (Express or full)  
- IDE: Visual Studio 2022, Rider, or VS Code with C# extension

---

## Getting Started

1. **Clone the repo:**
```bash
git clone https://github.com/anvex21/MovieReview.git
cd MovieReview
```

2. **Update connection string**
Edit appsettings.json to set your SQL Server connection string:
```bash
"ConnectionStrings": {
    "DefaultConnection": "Your-connection-string-here"
}
```

3. **Apply migrations and update database**

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

The response returns a **JWT token.** Include this token as Authorization: Bearer <token> for all protected endpoints.

---

## Notes
**Without a valid Bearer token, you can only access Register and Login.**

**CRUD operations for movies and reviews require authentication.**

**Filtering, sorting, and pagination work only on the GetAllMoviesWithQuery endpoint.**
