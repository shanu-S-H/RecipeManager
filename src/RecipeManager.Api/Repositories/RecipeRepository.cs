using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Data;
using RecipeManager.Api.Dtos;
using RecipeManager.Api.Models;

namespace RecipeManager.Api.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _db;

    public RecipeRepository(AppDbContext db) => _db = db;

    public async Task<PagedResult<RecipeDto>> GetPagedAsync(
        int userId, string? search, string? category, int page, int pageSize)
    {
        var query = _db.Recipes.AsNoTracking().Where(r => r.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Title.Contains(search) || r.Ingredients.Contains(search));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(r => r.Category == category);

        var total = await query.CountAsync();

        // Server-side pagination + DTO projection
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RecipeDto(r.Id, r.Title, r.Category, r.Ingredients, r.Instructions, r.CreatedAt))
            .ToListAsync();

        return new PagedResult<RecipeDto>(items, page, pageSize, total);
    }

    public Task<Recipe?> GetByIdAsync(int userId, int id) =>
        _db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

    public async Task<Recipe> AddAsync(Recipe recipe)
    {
        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();
        return recipe;
    }

    public async Task UpdateAsync(Recipe recipe)
    {
        _db.Recipes.Update(recipe);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Recipe recipe)
    {
        _db.Recipes.Remove(recipe);
        await _db.SaveChangesAsync();
    }
}
