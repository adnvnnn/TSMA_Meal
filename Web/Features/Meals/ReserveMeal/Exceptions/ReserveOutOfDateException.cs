using TSMA.Meal.Web.Shared.Exceptions;

namespace TSMA.Meal.Web.Features.Meals.ReserveMeal.Exceptions;

internal class ReserveOutOfDateException() : BaseException("زمان رزرو غذا به پایان رسیده است.");
