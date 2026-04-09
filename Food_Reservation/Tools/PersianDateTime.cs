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


    public static DayOfWeek Today => DateTime.Now.DayOfWeek + 2;

    public static DayOfWeek GetTodayWeekDay()
    {
        return Today;
    }
    public static string GetTodayPersianDate()
    {
        return GetPersianDateByWeekDay((int)Today);
    }

    public static string GetPersianDateByWeekDay(int day)
    {
        var daysToSaturday = Today == DayOfWeek.Sunday ? 6 : (int)Today;
        
        var startOfWeekDate = DateTime.Now.AddDays(-daysToSaturday).Date;

        var weekDays = new List<DateTime>
        {
            startOfWeekDate.AddDays(0),
            startOfWeekDate.AddDays(1),
            startOfWeekDate.AddDays(2),
            startOfWeekDate.AddDays(3),
            startOfWeekDate.AddDays(4),
            startOfWeekDate.AddDays(5),
            startOfWeekDate.AddDays(6)
        };

        return weekDays[day].ToPersianDate();
    }
}