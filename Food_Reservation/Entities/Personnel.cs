namespace TSMA_Meal.Entities;

public class Personnel
{
    public bool Active { get; set; }
    public decimal Discount { get; set; }
    public int PersonnelCode { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }
}