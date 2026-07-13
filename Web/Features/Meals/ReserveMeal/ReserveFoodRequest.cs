using MediatR;

namespace TSMA.Meal.Web.Features.Meals.ReserveMeal;

public record ReserveFoodRequest(int FoodId, int PersonnelCode, int Week) : IRequest<int>;