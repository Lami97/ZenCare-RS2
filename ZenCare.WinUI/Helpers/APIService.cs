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
    public string? LastErrorMessage { get; private set; }

    public async Task<T?> Get<T>(string endpoint)
    {
        AttachAuthorizationHeader();
        LastErrorMessage = null;

        var response = await Client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            LastErrorMessage = await ReadErrorMessage(response);
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> Post<T>(string endpoint, object request)
    {
        AttachAuthorizationHeader();
        LastErrorMessage = null;
        var response = await Client.PostAsJsonAsync(endpoint, request);

        if (!response.IsSuccessStatusCode)
        {
            LastErrorMessage = await ReadErrorMessage(response);
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> Put<T>(string endpoint, object request)
    {
        AttachAuthorizationHeader();
        LastErrorMessage = null;
        var response = await Client.PutAsJsonAsync(endpoint, request);

        if (!response.IsSuccessStatusCode)
        {
            LastErrorMessage = await ReadErrorMessage(response);
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> Delete(string endpoint)
    {
        AttachAuthorizationHeader();
        LastErrorMessage = null;
        var response = await Client.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            LastErrorMessage = await ReadErrorMessage(response);
            return false;
        }

        return true;
    }

    private void AttachAuthorizationHeader()
    {
        var token = Token ?? AuthStorage.Token;

        Client.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<string> ReadErrorMessage(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(content))
        {
            return content;
        }

        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Niste prijavljeni.",
            System.Net.HttpStatusCode.Forbidden => "Nemate pravo pristupa.",
            System.Net.HttpStatusCode.NotFound => "Trazeni podatak nije pronadjen.",
            System.Net.HttpStatusCode.Conflict => "Podatak je u konfliktu sa postojecim zapisima.",
            _ => $"Greska prilikom poziva API-ja: {(int)response.StatusCode}"
        };
    }
}
