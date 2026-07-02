using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ZenCare.WinUI.Helpers;

public class APIService
{
    private const string BaseUrl = "http://localhost:5281";
    private static readonly HttpClient Client = new HttpClient
    {
        BaseAddress = new Uri(BaseUrl)
    };

    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }

    public async Task<T?> Get<T>(string endpoint)
    {
        AttachAuthorizationHeader();
        return await Client.GetFromJsonAsync<T>(endpoint);
    }

    public async Task<T?> Post<T>(string endpoint, object request)
    {
        AttachAuthorizationHeader();
        var response = await Client.PostAsJsonAsync(endpoint, request);

        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> Put<T>(string endpoint, object request)
    {
        AttachAuthorizationHeader();
        var response = await Client.PutAsJsonAsync(endpoint, request);

        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task Delete(string endpoint)
    {
        AttachAuthorizationHeader();
        await Client.DeleteAsync(endpoint);
    }

    private void AttachAuthorizationHeader()
    {
        var token = Token ?? AuthStorage.Token;

        Client.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }
}
