using Food_Reservation.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Food_Reservation.Database.Configs;

public class PersonnelInfoConfig : IEntityTypeConfiguration<PersonnelInfo>
{
    public void Configure(EntityTypeBuilder<PersonnelInfo> builder)
    {
        builder.ToTable("InformationTbl");
    }
}