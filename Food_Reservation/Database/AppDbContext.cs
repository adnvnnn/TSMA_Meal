using Microsoft.EntityFrameworkCore;
using TSMA_Meal.Database.Entities;

namespace TSMA_Meal.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(){}
    public AppDbContext(DbContextOptions<AppDbContext> op) : base(op) { }


    public DbSet<Meal> Meals { get; set; }
    public DbSet<MealOffDay> MealOffDay { get; set; }
    public DbSet<Reserve> Reserve { get; set; }
    public DbSet<Personnel> Personnel { get; set; }
    public DbSet<PersonnelInfo> PersonnelInfo { get; set; }
    public DbSet<PersonnelAccess> PersonnelAccess { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}