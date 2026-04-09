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



    public static int Today => GetPersianDayOfWeek(DateTime.Now.DayOfWeek);

    public static string GetTodayPersianDate()
    {
        return GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);
    }


    public static int GetPersianDayOfWeek(DayOfWeek dayOfWeek)
    {
        int standardIndex = (int)dayOfWeek;
        return (standardIndex + 1) % 7;
    }



    public static string GetPersianDateByWeekDay(int day, int weekOffset = 0)
    {
        var daysToSaturday = Today == (int)DayOfWeek.Friday ? 6 : Today;
        var startOfWeekDate = DateTime.Now.AddDays(-daysToSaturday + (weekOffset * 7)).Date;

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



    public static string GetPersianWeekStartDate(int weekOffset)
    {

        var daysToSaturday = Today == (int)DayOfWeek.Friday ? 6 : Today;

        var currentWeekStart = DateTime.Now.AddDays(-daysToSaturday).Date;
        var weekStartDate = currentWeekStart.AddDays(weekOffset * 7);
        return weekStartDate.ToPersianDate();
    }
    public static string GetPersianWeekEndDate(int weekOffset)
    {

        var daysToSaturday = Today == (int)DayOfWeek.Friday ? 6 : Today; var currentWeekStart = DateTime.Now.AddDays(-daysToSaturday).Date;
        var weekEndDate = currentWeekStart.AddDays(weekOffset * 7 + 6);
        return weekEndDate.ToPersianDate();
    }
}