using Food_Reservation.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food_Reservation.Database.Configs;

public class ReserveConfig : IEntityTypeConfiguration<Reserve>
{
    public void Configure(EntityTypeBuilder<Reserve> builder)
    {
        builder.ToTable("MealReservedWeeklyAppTbl");

        builder.Property(f => f.PersonnelCode).HasColumnName("PersonalCode");

        builder.Property(f => f.ReserveDate).HasColumnName("DateReserve");

        builder.Property(f => f.FoodId).HasColumnName("FoodID");

    }
}