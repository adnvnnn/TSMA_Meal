using Food_Reservation.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food_Reservation.Database.Configs;

public class MealOffDayConfig : IEntityTypeConfiguration<MealOffDay>
{
    public void Configure(EntityTypeBuilder<MealOffDay> builder)
    {
        builder.ToTable("MealOffDayAppTbl");

        builder.HasKey(u => u.Id);

        builder.Property(f => f.Id).HasColumnName("ID");

        builder.Property(f => f.Date).HasColumnName("DateOff");

    }
}