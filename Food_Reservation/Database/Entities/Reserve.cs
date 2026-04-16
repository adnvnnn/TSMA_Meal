using TSMA_Meal.Database.Entities.Common;

namespace TSMA_Meal.Database.Entities;

public class Reserve : BaseEntity
{
    public int PersonnelCode { get; set; }
    public int MealId { get; set; }
    public string ReserveDate { get; set; }

    public string? DayName { get; set; }
    public string? PaymentOrder { get; set; }
    public string? PriceFood { get; set; }
    public string? PriceForUser { get; set; }
    public string? PercentOff { get; set; }
    public string? WaitForValid { get; set; }
    public bool CanceledReserve { get; set; }
    public string? CausedCancel { get; set; }
    public string? TimeReserve { get; set; }
    public bool? TypeReserve { get; set; }
    public string? MACRegisteration { get; set; }

}