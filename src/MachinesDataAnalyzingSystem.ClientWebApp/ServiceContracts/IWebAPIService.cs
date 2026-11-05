namespace MachinesDataAnalyzingSystem.ClientWebApp.ServiceContracts;

public interface IWebAPIService
{
    Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string endpoint);
    Task<HttpResponseMessage> SendAsync<T>(HttpMethod httpMethod, string endpoint, T value);
    Task<HttpResponseMessage> SendAsyncWithoutAuthorizationHeader<T>(HttpMethod httpMethod, string endpoint, T value);
}