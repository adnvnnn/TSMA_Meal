using MediatR;

namespace TSMA_Meal.Features.Meals.MealsList;

public record MealsListRequest(int PersonnelCode, int PageId = 0) : IRequest<List<MealsListViewModel>>;