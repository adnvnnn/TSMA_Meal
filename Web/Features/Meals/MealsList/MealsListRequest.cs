using MediatR;

namespace TSMA.Meal.Web.Features.Meals.MealsList;

public record MealsListRequest(int PersonnelCode, int PageId = 0) : IRequest<List<MealsListViewModel>>;