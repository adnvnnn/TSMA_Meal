using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSMA.Meal.Web.Database.Entities;

namespace TSMA.Meal.Web.Database.Configs;

public class PersonnelInfoConfig : IEntityTypeConfiguration<PersonnelInfo>
{
    public void Configure(EntityTypeBuilder<PersonnelInfo> builder)
    {
        builder.ToTable("InformationTbl");

        builder.Property(f => f.Phone).HasColumnName("mobile");

    }
}