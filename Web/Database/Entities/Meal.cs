using TSMA.Meal.Web.Database.Entities.Common;

namespace TSMA.Meal.Web.Database.Entities;

public class Meal : BaseEntity
{
    public int OrderDay { get; set; }
    public int OrderListDay { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string ImageUrl2 { get; set; }
    public string Restaurant { get; set; }
    public string Day { get; set; }
    public string Price { get; set; }
}