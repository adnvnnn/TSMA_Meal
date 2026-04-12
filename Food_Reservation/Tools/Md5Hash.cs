using System.Security.Cryptography;
using System.Text;

namespace TSMA_Meal.Tools;

public static class Md5Hash
{
    public static string Hash(string pass)
    {
        using MD5 md5 = MD5.Create();
        byte[] originalBytes = Encoding.UTF8.GetBytes(pass);
        byte[] encodedBytes = md5.ComputeHash(originalBytes);
        return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
    }
}