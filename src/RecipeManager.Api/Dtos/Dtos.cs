using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Api.Dtos;

public record RegisterRequest(
    [Required] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);

public record AuthResponse(string Token, string FullName, string Email);

public record RecipeRequest(
    [Required, MaxLength(150)] string Title,
    [Required, MaxLength(50)] string Category,
    [Required] string Ingredients,
    [Required] string Instructions);

public record RecipeDto(int Id, string Title, string Category, string Ingredients, string Instructions, DateTime CreatedAt);

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);

public record MealPlanRequest([Required] int RecipeId, [Required] DateOnly Date, [Required] string MealType);

public record MealPlanDto(int Id, int RecipeId, string RecipeTitle, DateOnly Date, string MealType);
