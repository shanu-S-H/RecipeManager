using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Dtos;
using RecipeManager.Api.Models;
using RecipeManager.Api.Repositories;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeRepository _repo;

    public RecipesController(IRecipeRepository repo) => _repo = repo;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/recipes?search=pasta&category=Dinner&page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<RecipeDto>>> GetAll(
        string? search, string? category, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);
        return Ok(await _repo.GetPagedAsync(UserId, search, category, page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeDto>> GetById(int id)
    {
        var r = await _repo.GetByIdAsync(UserId, id);
        return r is null ? NotFound() : Ok(ToDto(r));
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Create(RecipeRequest request)
    {
        var recipe = new Recipe
        {
            UserId = UserId,
            Title = request.Title.Trim(),
            Category = request.Category.Trim(),
            Ingredients = request.Ingredients.Trim(),
            Instructions = request.Instructions.Trim()
        };

        await _repo.AddAsync(recipe);
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, ToDto(recipe));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RecipeRequest request)
    {
        var recipe = await _repo.GetByIdAsync(UserId, id);
        if (recipe is null) return NotFound();

        recipe.Title = request.Title.Trim();
        recipe.Category = request.Category.Trim();
        recipe.Ingredients = request.Ingredients.Trim();
        recipe.Instructions = request.Instructions.Trim();

        await _repo.UpdateAsync(recipe);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var recipe = await _repo.GetByIdAsync(UserId, id);
        if (recipe is null) return NotFound();

        await _repo.DeleteAsync(recipe);
        return NoContent();
    }

    private static RecipeDto ToDto(Recipe r) =>
        new(r.Id, r.Title, r.Category, r.Ingredients, r.Instructions, r.CreatedAt);
}
