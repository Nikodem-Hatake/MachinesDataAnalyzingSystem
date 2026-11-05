using MachinesDataAnalyzingSystem.DataCollector.ServiceContracts;
using System.Text.Json;

namespace MachinesDataAnalyzingSystem.DataCollector;

public class Worker(ILogger<Worker> logger, IMachineDataCollector machineDataCollector, 
    IQueueDataAdder queueDataAdder) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            var datas = machineDataCollector.CollectNewData();
            logger.LogInformation($"Collected data at {DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss")}.");

            foreach(var data in datas)
            {
                await queueDataAdder.AddMessage(JsonSerializer.Serialize(data));
            }
            logger.LogInformation($"Added data to queue at {DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss")}.");

            await Task.Delay(1500);
        }
    }
}
