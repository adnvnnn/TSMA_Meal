using Food_Reservation.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food_Reservation.Database.Configs;

public class PersonnelAccessConfig : IEntityTypeConfiguration<PersonnelAccess>
{
    public void Configure(EntityTypeBuilder<PersonnelAccess> builder)
    {
        builder.ToTable("AccessiBetaAppTbl");

        builder.HasKey(u => u.Id);

        builder.Property(f => f.Id).HasColumnName("ID");

        builder.Property(f => f.FoodReservation).HasColumnName("ReserveGhaza");

        builder.Property(f => f.PersonnelCode).HasColumnName("IDPersonal");

    }
}