namespace TSMA_Meal.Features.Meals.ReserveMeal;

internal class ReserveOutOfDateException() : Exception("زمان رزرو غذا به پایان رسیده است.");
internal class ReserveIsOffException() : Exception("رزرو امکان پذیر نمیباشد.");