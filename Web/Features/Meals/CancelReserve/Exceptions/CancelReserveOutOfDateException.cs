using TSMA.Meal.Web.Shared.Exceptions;

namespace TSMA.Meal.Web.Features.Meals.CancelReserve.Exceptions;

internal class CancelReserveOutOfDateException() : BaseException("زمان لغو رزرو غذا به پایان رسیده است.");
