# Recipe Manager API

A secure REST API to save, organize, search and plan meals from your favorite recipes.
Built with **ASP.NET Core 8 Web API**, **Entity Framework Core**, **SQL Server** and **JWT authentication**.

## Features
- User registration and login with **JWT** authentication (passwords hashed with BCrypt)
- Recipe CRUD with categories, **search**, filtering and **server-side pagination**
- Weekly **meal planning** (breakfast / lunch / dinner / snack per date)
- Auto-generated, de-duplicated **shopping list** from planned meals
- **Repository pattern**, dependency injection, global exception-handling middleware
- Swagger UI with JWT support

## Tech stack
ASP.NET Core 8 · C# · Entity Framework Core · SQL Server · JWT · Swagger · Repository pattern

## Project structure
```
src/RecipeManager.Api
├── Controllers   (Auth, Recipes, MealPlans)
├── Data          (AppDbContext)
├── Dtos          (request/response models)
├── Middleware    (global exception handling)
├── Models        (User, Recipe, MealPlan)
├── Repositories  (IRecipeRepository, RecipeRepository)
└── Services      (JwtTokenService)
```

## Getting started
Prerequisites: .NET 8 SDK, SQL Server (LocalDB or full).

```bash
cd src/RecipeManager.Api

# 1. Update the connection string and Jwt:Key in appsettings.json
# 2. Create the database
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update

# 3. Run
dotnet run
```
Open `https://localhost:<port>/swagger`, register a user, copy the token, click **Authorize**
and enter `Bearer <token>`.

## API endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/register | Create an account |
| POST | /api/auth/login | Get a JWT |
| GET | /api/recipes?search=&category=&page=&pageSize= | List recipes (paged) |
| GET | /api/recipes/{id} | Get one recipe |
| POST | /api/recipes | Create a recipe |
| PUT | /api/recipes/{id} | Update a recipe |
| DELETE | /api/recipes/{id} | Delete a recipe |
| GET | /api/mealplans?from=&to= | Meal plan for a date range |
| POST | /api/mealplans | Add a recipe to a date |
| DELETE | /api/mealplans/{id} | Remove a planned meal |
| GET | /api/mealplans/shopping-list?from=&to= | Shopping list for a date range |

## Roadmap
- Angular 16 front end (recipe list, meal-planning calendar, shopping list)
- Unit tests with xUnit
- Docker support

## Author
Shubhangi Sharma — [LinkedIn](https://www.linkedin.com/in/shubhangi-sharma-6350ba213)
