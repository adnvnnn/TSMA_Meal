using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSMA_Meal.Database.Entities;

namespace TSMA_Meal.Database.Configs;

public class PersonnelInfoConfig : IEntityTypeConfiguration<PersonnelInfo>
{
    public void Configure(EntityTypeBuilder<PersonnelInfo> builder)
    {
        builder.ToTable("InformationTbl");

        builder.Property(f => f.Phone).HasColumnName("mobile");

    }
}