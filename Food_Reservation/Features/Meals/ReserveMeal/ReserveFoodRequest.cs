using MediatR;

namespace TSMA_Meal.Features.Meals.ReserveMeal;

public record ReserveFoodRequest(int FoodId, int PersonnelCode, int Week) : IRequest<int>;