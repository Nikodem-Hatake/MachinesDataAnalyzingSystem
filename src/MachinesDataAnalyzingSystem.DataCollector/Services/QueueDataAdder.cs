using MachinesDataAnalyzingSystem.DataCollector.ServiceContracts;
using RabbitMQ.Client;
using System.Text;

namespace MachinesDataAnalyzingSystem.DataCollector.Services;

public class QueueDataAdder : IQueueDataAdder
{
    private readonly IChannel? _channel;
    private readonly IConnection? _connection;
    
    public QueueDataAdder(IConfiguration configuration)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = configuration.GetConnectionString("RabbitMQ"),
            Password = configuration["RabbitMQConnectionInfo:Password"],
            Port = int.Parse(configuration["RabbitMQConnectionInfo:Port"]),
            UserName = configuration["RabbitMQConnectionInfo:UserName"]
        };
        _connection = connectionFactory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.QueueDeclareAsync(configuration["RabbitMQConnectionInfo:QueueName"], true, false, false).Wait();
    }

    public async Task AddMessage(string message)
    {
        await _channel.BasicPublishAsync("", _channel.CurrentQueue, Encoding.UTF8.GetBytes(message));
    }
}