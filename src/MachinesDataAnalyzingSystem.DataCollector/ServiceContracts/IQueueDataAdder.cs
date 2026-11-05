namespace MachinesDataAnalyzingSystem.DataCollector.ServiceContracts;

public interface IQueueDataAdder
{
    Task AddMessage(string message);
}