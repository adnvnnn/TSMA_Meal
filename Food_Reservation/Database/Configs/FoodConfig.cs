using Food_Reservation.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food_Reservation.Database.Configs;

public class FoodConfig : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable("RestaurantAppTbl");

        builder.HasKey(u => u.Id);

        builder.Property(f => f.Id).HasColumnName("ID");

        builder.Property(f => f.Name).HasColumnName("FoodName");

        builder.Property(f => f.ImageUrl).HasColumnName("FoodImageUrl");

        builder.Property(f => f.Restaurant).HasColumnName("RestaurantName");

        builder.Property(f => f.Day).HasColumnName("DayPersianName");

        builder.Property(f => f.Price).HasColumnName("Price");

        builder.Property(f => f.OrderDay).HasColumnName("OrderListDay");


    }
}