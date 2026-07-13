using System.Text;

namespace TSMA.Meal.Web.Tools;

public static class Encoder
{
    public static string Encode(string username)
    {
        if (string.IsNullOrEmpty(username)) return string.Empty;
        var plainTextBytes = Encoding.UTF8.GetBytes(username);
        return Convert.ToBase64String(plainTextBytes);
    }
}