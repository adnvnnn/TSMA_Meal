namespace TSMA_Meal.Shared;

public interface IAuthenticationApiService
{
    Task Login(string phoneNumber,string code);

    Task Logout();
}

internal class AuthenticationApiService(HttpClient http) : IAuthenticationApiService
{
    public async Task Login(string phoneNumber, string code)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/check-otp")
        {
            Content = JsonContent.Create(new { Code = code, PhoneNumber = phoneNumber })
        };

        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error ?? "Error");
        }

    }

    public async Task Logout()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/logout");

        await http.SendAsync(request);
    }
}