using Microsoft.EntityFrameworkCore;
using TSMA.Meal.Web.Database;
using TSMA.Meal.Web.Database.Entities;

namespace TSMA.Meal.Web.Setup;

/// <summary>
/// Fills the demo (in-memory) database with a single personnel record and a working
/// week of meals (Saturday to Thursday, Friday is a weekend day) so the application
/// can be used without the legacy database.
/// </summary>
public static class DemoDataSeeder
{
    public const string DemoPhone = "09120000000";
    public const int DemoPersonnelCode = 1001;

    public static void SeedDemoData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();

        if (context.Meals.Any() || context.Personnel.Any()) return;

        context.Personnel.Add(new Personnel
        {
            PersonnelCode = DemoPersonnelCode,
            Name = "کاربر",
            LastName = "نمایشی",
            Discount = 20,
            Active = true
        });

        context.PersonnelInfo.Add(new PersonnelInfo
        {
            Id = DemoPersonnelCode,
            Phone = DemoPhone
        });

        context.PersonnelAccess.Add(new PersonnelAccess
        {
            PersonnelCode = DemoPersonnelCode,
            FoodReservation = true
        });

        context.Meals.AddRange(BuildWeekMeals());

        context.SaveChanges();
    }

    private static IEnumerable<Database.Entities.Meal> BuildWeekMeals()
    {
        // OrderDay follows System.DayOfWeek (Sunday = 0 ... Saturday = 6),
        // OrderListDay keeps the Persian week order starting on Saturday.
        // ImageUrl / ImageUrl2 are the two dishes of the day; the listing picks
        // between them by week parity, so each one is the photo of its own dish.
        var week = new (string PersianDay, int OrderDay, string Name, string ImageUrl, string ImageUrl2)[]
        {
            ("شنبه", (int)DayOfWeek.Saturday,
                "چلوکباب کوبیده / جوجه‌کباب",
                "/images/meals/kabab-koobideh.jpg", "/images/meals/joojeh-kabab.jpg"),
            ("یکشنبه", (int)DayOfWeek.Sunday,
                "قیمه نثار / زرشک‌پلو با مرغ",
                "/images/meals/gheymeh-nesar.jpg", "/images/meals/zereshk-polo-morgh.jpg"),
            ("دوشنبه", (int)DayOfWeek.Monday,
                "خورش قیمه / خورش قورمه‌سبزی",
                "/images/meals/khoresh-gheymeh.jpg", "/images/meals/ghormeh-sabzi.jpg"),
            ("سه‌شنبه", (int)DayOfWeek.Tuesday,
                "چلو مرغ / کباب کوبیده",
                "/images/meals/chelow-morgh.jpg", "/images/meals/kabab-koobideh-2.jpg"),
            ("چهارشنبه", (int)DayOfWeek.Wednesday,
                "آبگوشت / عدس‌پلو",
                "/images/meals/abgoosht.jpg", "/images/meals/adas-polo.jpg"),
            ("پنجشنبه", (int)DayOfWeek.Thursday,
                "ماهی قزل‌آلا / کوکو سبزی",
                "/images/meals/mahi-ghzel-ala.jpg", "/images/meals/kookoo-sabzi.jpg")
        };

        for (var i = 0; i < week.Length; i++)
        {
            yield return new Database.Entities.Meal
            {
                OrderDay = week[i].OrderDay,
                OrderListDay = i + 1,
                Name = week[i].Name,
                ImageUrl = week[i].ImageUrl,
                ImageUrl2 = week[i].ImageUrl2,
                Restaurant = "رستوران نمایشی",
                Day = week[i].PersianDay,
                Price = "150000"
            };
        }
    }
}
