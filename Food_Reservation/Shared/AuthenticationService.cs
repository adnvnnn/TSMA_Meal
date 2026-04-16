namespace TSMA_Meal.Shared;

public interface IAuthenticationApiService
{
    Task<bool> Login(string phoneNumber,string code);

    Task Logout();
}

internal class AuthenticationApiService(HttpClient http) : IAuthenticationApiService
{
    public async Task<bool> Login(string phoneNumber, string code)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/check-login")
        {
            Content = JsonContent.Create(new { Code = code, PhoneNumber = phoneNumber })
        };

        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error ?? "Error");
        }

        return true;
    }

    public async Task Logout()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/logout");

        await http.SendAsync(request);
    }
}