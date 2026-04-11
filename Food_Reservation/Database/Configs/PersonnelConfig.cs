using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSMA_Meal.Entities;

namespace TSMA_Meal.Database.Configs;

public class PersonnelConfig : IEntityTypeConfiguration<Personnel>
{
    public void Configure(EntityTypeBuilder<Personnel> builder)
    {
        builder.ToTable("NamesTbl");

        builder.Property(u => u.PersonnelCode).HasColumnName("PersonalCode");

        builder.HasKey(u => u.PersonnelCode);

        builder.Property(u => u.Discount).HasColumnName("MealOffPercent");

        builder.Property(u => u.Active)
            .HasColumnName("Boxworking")
            .HasConversion(
                v => v ? "true" : "false",   
                v => v.ToLower() == "true" || v == "1"  
            );

        builder.HasQueryFilter(p => p.Active);
    }
}