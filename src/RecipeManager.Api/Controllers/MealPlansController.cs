using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Data;
using RecipeManager.Api.Dtos;
using RecipeManager.Api.Models;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MealPlansController : ControllerBase
{
    private static readonly string[] ValidMealTypes = { "Breakfast", "Lunch", "Dinner", "Snack" };
    private readonly AppDbContext _db;

    public MealPlansController(AppDbContext db) => _db = db;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/mealplans?from=2026-10-19&to=2026-10-25
    [HttpGet]
    public async Task<ActionResult<List<MealPlanDto>>> GetRange(DateOnly from, DateOnly to)
    {
        var plans = await _db.MealPlans.AsNoTracking()
            .Where(m => m.UserId == UserId && m.Date >= from && m.Date <= to)
            .OrderBy(m => m.Date).ThenBy(m => m.MealType)
            .Select(m => new MealPlanDto(m.Id, m.RecipeId, m.Recipe!.Title, m.Date, m.MealType))
            .ToListAsync();

        return Ok(plans);
    }

    [HttpPost]
    public async Task<ActionResult<MealPlanDto>> Create(MealPlanRequest request)
    {
        if (!ValidMealTypes.Contains(request.MealType))
            return BadRequest(new { message = $"MealType must be one of: {string.Join(", ", ValidMealTypes)}" });

        var recipe = await _db.Recipes.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RecipeId && r.UserId == UserId);
        if (recipe is null) return NotFound(new { message = "Recipe not found." });

        var plan = new MealPlan
        {
            UserId = UserId,
            RecipeId = recipe.Id,
            Date = request.Date,
            MealType = request.MealType
        };

        _db.MealPlans.Add(plan);
        await _db.SaveChangesAsync();

        return Ok(new MealPlanDto(plan.Id, recipe.Id, recipe.Title, plan.Date, plan.MealType));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _db.MealPlans.FirstOrDefaultAsync(m => m.Id == id && m.UserId == UserId);
        if (plan is null) return NotFound();

        _db.MealPlans.Remove(plan);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET api/mealplans/shopping-list?from=...&to=...
    // Builds a de-duplicated shopping list from all planned recipes in the date range
    [HttpGet("shopping-list")]
    public async Task<ActionResult<List<string>>> ShoppingList(DateOnly from, DateOnly to)
    {
        var ingredientBlocks = await _db.MealPlans.AsNoTracking()
            .Where(m => m.UserId == UserId && m.Date >= from && m.Date <= to)
            .Select(m => m.Recipe!.Ingredients)
            .ToListAsync();

        var items = ingredientBlocks
            .SelectMany(block => block.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(i => i)
            .ToList();

        return Ok(items);
    }
}
