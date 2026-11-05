using MachinesDataAnalyzingSystem.DataAdder.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared;
using RabbitMQ.Client.Events;
using System.Text;

namespace MachinesDataAnalyzingSystem.DataAdder.Services;

public class DataAdderToAPI(IConfiguration configuration, ILogger<DataAdderToAPI> logger) : IDataAdder
{
    public async Task AddData(object obj, BasicDeliverEventArgs eventArgs)
    {
        logger.LogInformation($"Data recieved at {DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss")}.");
        var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, configuration.GetConnectionString("WebAPIPostMachineDataEndpoint"));
        request.Content = new StringContent(message, mediaType: new("application/json"));
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, configuration["APIKey"]);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        logger.LogInformation($"Data sended to web API at {DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss")}.");
    }
}