using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Features.Meals.CancelReserve.Exceptions;

internal class CancelReserveOutOfDateException() : BaseException("زمان لغو رزرو غذا به پایان رسیده است.");
