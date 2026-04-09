using System.Globalization;

namespace Food_Reservation.Tools;

public static class PersianDateTime
{
    public static string ToPersianDate(this DateTime date)
    {
        PersianCalendar pc = new PersianCalendar();
        return $"{pc.GetYear(date):0000}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00}";
    }

    public static string ToPersianDateTime(this DateTime dateTime)
    {
        var persianCalendar = new PersianCalendar();
        var year = persianCalendar.GetYear(dateTime);
        var month = persianCalendar.GetMonth(dateTime);
        var day = persianCalendar.GetDayOfMonth(dateTime);
        var hour = dateTime.Hour;
        var minute = dateTime.Minute;
        var second = dateTime.Second;

        return $"{year:D4}/{month:D2}/{day:D2} {hour:D2}:{minute:D2}:{second:D2}";
    }
}