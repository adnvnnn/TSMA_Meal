using TSMA_Meal.Database.Entities.Common;

namespace TSMA_Meal.Database.Entities;

public class PersonnelAccess : BaseEntity
{
    public bool FoodReservation { get; set; }
    public int PersonnelCode { get; set; }
}