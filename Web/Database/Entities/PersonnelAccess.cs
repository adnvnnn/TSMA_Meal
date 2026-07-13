using TSMA.Meal.Web.Database.Entities.Common;

namespace TSMA.Meal.Web.Database.Entities;

public class PersonnelAccess : BaseEntity
{
    public bool FoodReservation { get; set; }
    public int PersonnelCode { get; set; }
}