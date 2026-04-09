namespace Food_Reservation.Tools;

public static class PriceFormat
{
    public static string FormatNumber(string num)
    {
        if (string.IsNullOrEmpty(num)) return num;

        var parts = num.Split('.');
        var integerPart = parts[0];
        var formattedInteger = System.Text.RegularExpressions.Regex
            .Replace(integerPart, @"(\d)(?=(\d{3})+(?!\d))", "$1,");

        return parts.Length > 1 ? $"{formattedInteger}.{parts[1]}" : formattedInteger;
    }
}