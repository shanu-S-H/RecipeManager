using RecipeManager.Api.Dtos;
using RecipeManager.Api.Models;

namespace RecipeManager.Api.Repositories;

public interface IRecipeRepository
{
    Task<PagedResult<RecipeDto>> GetPagedAsync(int userId, string? search, string? category, int page, int pageSize);
    Task<Recipe?> GetByIdAsync(int userId, int id);
    Task<Recipe> AddAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(Recipe recipe);
}
