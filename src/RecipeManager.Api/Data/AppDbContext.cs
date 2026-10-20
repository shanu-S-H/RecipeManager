using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Models;

namespace RecipeManager.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Recipe>(e =>
        {
            e.HasIndex(r => new { r.UserId, r.Category });
            e.HasOne(r => r.User).WithMany(u => u.Recipes).HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MealPlan>(e =>
        {
            e.HasIndex(m => new { m.UserId, m.Date });
            e.HasOne(m => m.Recipe).WithMany().HasForeignKey(m => m.RecipeId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
