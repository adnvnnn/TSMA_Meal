using Food_Reservation.Entities;
using Microsoft.EntityFrameworkCore;

namespace Food_Reservation.Database;

public class AppDbContext : DbContext
{
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Reserve> Reserve { get; set; }
    public DbSet<PersonnelInfo> PersonnelInfo { get; set; }
    public DbSet<Personnel> Personnel { get; set; }
    public DbSet<MealOffDay> MealOffDay { get; set; }

    public AppDbContext()
    {

    }

    public AppDbContext(DbContextOptions<AppDbContext> op) : base(op)
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>().ToTable("RestaurantAppTbl");
        modelBuilder.Entity<Reserve>().ToTable("MealReservedWeeklyAppTbl");
        modelBuilder.Entity<PersonnelInfo>().ToTable("InformationTbl");
        modelBuilder.Entity<Personnel>().ToTable("NamesTbl");
        modelBuilder.Entity<MealOffDay>().ToTable("MealOffDayAppTbl");

        base.OnModelCreating(modelBuilder);
    }
}