namespace TSMA_Meal.Shared;

public static class LogMessages
{
    public static string UserTriedReserveWrongMeal(int personnelCode) => $"Personnel {personnelCode} attempted to reserve incorrect meal";

    public static string WrongUserToReserveMeal(int wrongPersonnelCode) => $"Unauthorized reservation attempt by personnel {wrongPersonnelCode} - meal mismatch";

    public static string WrongUserToCancelMeal(int wrongPersonnelCode) => $"Personnel {wrongPersonnelCode} attempted to cancel meal without ownership";

    public static string UserTriedCancelWrongMeal(int personnelCode) =>$"Personnel {personnelCode} tried to cancel non-existent or wrong meal";

    public static string UserTriedCancelOthersMeal(int personnelCode, int otherPersonnelCode) => $"Personnel {personnelCode} attempted to cancel meal belonging to {otherPersonnelCode}";
}