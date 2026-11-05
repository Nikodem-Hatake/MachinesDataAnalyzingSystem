using RabbitMQ.Client.Events;

namespace MachinesDataAnalyzingSystem.DataAdder.ServiceContracts;

public interface IQueueDataReciever
{
    void AddReceivedAsync(AsyncEventHandler<BasicDeliverEventArgs> onReceivedAsync);
}