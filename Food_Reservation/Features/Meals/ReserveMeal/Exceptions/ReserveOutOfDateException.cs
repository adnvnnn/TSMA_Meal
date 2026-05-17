using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Features.Meals.ReserveMeal.Exceptions;

internal class ReserveOutOfDateException() : BaseException("زمان رزرو غذا به پایان رسیده است.");
