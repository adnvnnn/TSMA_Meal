using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSMA_Meal.Database.Entities;

namespace TSMA_Meal.Database.Configs;

public class MealConfig : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("RestaurantAppTbl");

        builder.HasKey(u => u.Id);

        builder.Property(f => f.Id).HasColumnName("ID");

        builder.Property(f => f.Name).HasColumnName("FoodName");

        builder.Property(f => f.ImageUrl).HasColumnName("FoodImageUrl");

        builder.Property(f => f.ImageUrl2).HasColumnName("FoodImageUrl2");

        builder.Property(f => f.Restaurant).HasColumnName("RestaurantName");

        builder.Property(f => f.Day).HasColumnName("DayPersianName");

        builder.Property(f => f.Price).HasColumnName("Price");

        builder.Property(f => f.OrderDay).HasColumnName("OrderDay");

        builder.Property(f => f.OrderListDay).HasColumnName("OrderListDay");

    }
}