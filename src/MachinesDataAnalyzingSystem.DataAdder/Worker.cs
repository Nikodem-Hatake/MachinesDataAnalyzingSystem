using MachinesDataAnalyzingSystem.DataAdder.ServiceContracts;

namespace MachinesDataAnalyzingSystem.DataAdder;

public class Worker(IQueueDataReciever queueDataReciever, IDataAdder dataAdder) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queueDataReciever.AddReceivedAsync(dataAdder.AddData);
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
    }
}
