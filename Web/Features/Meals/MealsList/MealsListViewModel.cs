namespace TSMA.Meal.Web.Features.Meals.MealsList;

public class MealsListViewModel
{
    public bool IsReserved { get; set; }
    public bool IsExpired { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsLoading { get; set; }
    public int ReserveId { get; set; }


    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string Restaurant { get; set; }
    public int OrderDay { get; set; }
    public string Day { get; set; }
    public string Date { get; set; }
    public string Price { get; set; }
    public decimal DiscountedPrice { get; set; }
}