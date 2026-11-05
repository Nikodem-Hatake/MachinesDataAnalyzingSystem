using RabbitMQ.Client.Events;

namespace MachinesDataAnalyzingSystem.DataAdder.ServiceContracts;

public interface IDataAdder
{
    Task AddData(object obj, BasicDeliverEventArgs eventArgs);
}