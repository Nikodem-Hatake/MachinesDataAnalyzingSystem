using MachinesDataAnalyzingSystem.ClientWebApp.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Accounts;
using Microsoft.JSInterop;
using System.Text.Json;

namespace MachinesDataAnalyzingSystem.ClientWebApp.Services;

public class WebAPIService(IHttpClientFactory httpClientFactory, IJSRuntime JSRuntime) : IWebAPIService
{
    private async Task GetNewJWTToken()
    {
        using var client = httpClientFactory.CreateClient("WebAPI");
        var response = await client.PostAsJsonAsync("/api/accounts/GenerateJWTToken", new JWTTokenRequest()
        {
            JWTRefreshToken = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "JWTRefreshToken"),
            JWTToken = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "JWTToken")
        });
        response.EnsureSuccessStatusCode();

        var authenticationResponse = JsonSerializer.Deserialize<AuthenticationResponse>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "JWTToken", authenticationResponse.JWTToken);
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "JWTTokenExpirationDate", 
            authenticationResponse.JWTTokenExpirationDate.ToString());
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "JWTRefreshToken", authenticationResponse.JWTRefreshToken);
    }

    private async Task<bool> IsJWTTokenExpired()
    {
        if(!DateTime.TryParse(await JSRuntime.InvokeAsync<string>("localStorage.getItem", "JWTTokenExpirationDate"), out var expirationDate))
        {
            return false;
        }

        return expirationDate < DateTime.UtcNow.AddMinutes(1);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string endpoint)
    {
        if(await IsJWTTokenExpired())
        {
            await GetNewJWTToken();
        }

        var request = new HttpRequestMessage(httpMethod, endpoint);
        await SetAuthorizationHeader(request);

        using var client = httpClientFactory.CreateClient("WebAPI");
        return await client.SendAsync(request);
    }

    public async Task<HttpResponseMessage> SendAsync<T>(HttpMethod httpMethod, string endpoint, T value)
    {
        if(await IsJWTTokenExpired())
        {
            await GetNewJWTToken();
        }

        var request = new HttpRequestMessage(httpMethod, endpoint)
        {
            Content = JsonContent.Create(value)
        };
        await SetAuthorizationHeader(request);

        using var client = httpClientFactory.CreateClient("WebAPI");
        return await client.SendAsync(request);
    }

    public async Task<HttpResponseMessage> SendAsyncWithoutAuthorizationHeader<T>(HttpMethod httpMethod, string endpoint, T value)
    {
        var request = new HttpRequestMessage(httpMethod, endpoint)
        {
            Content = JsonContent.Create(value)
        };

        using var client = httpClientFactory.CreateClient("WebAPI");
        return await client.SendAsync(request);
    }

    private async Task SetAuthorizationHeader(HttpRequestMessage request)
    {
        request.Headers.Authorization = new("Bearer", await JSRuntime.InvokeAsync<string>("localStorage.getItem", "JWTToken"));
    }
}