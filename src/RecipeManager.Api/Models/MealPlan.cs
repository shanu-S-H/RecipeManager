namespace RecipeManager.Api.Models;

public class MealPlan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
    public DateOnly Date { get; set; }
    // Breakfast, Lunch, Dinner, Snack
    public string MealType { get; set; } = "Dinner";
}
