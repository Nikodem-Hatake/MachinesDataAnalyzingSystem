using MachinesDataAnalyzingSystem.DataAdder.ServiceContracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MachinesDataAnalyzingSystem.DataAdder.Services;

public class QueueDataReciever : IQueueDataReciever
{
    private readonly IChannel _channel;
    private readonly IConnection _connection;
    private readonly AsyncEventingBasicConsumer _consumer;
    public IChannel Channel => _channel;

    public QueueDataReciever(IConfiguration configuration)
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

        _consumer = new(_channel);
        _channel.BasicConsumeAsync(configuration["RabbitMQConnectionInfo:QueueName"], false, _consumer).Wait();
    }

    public void AddReceivedAsync(AsyncEventHandler<BasicDeliverEventArgs> onReceivedAsync)
    {
        _consumer.ReceivedAsync += onReceivedAsync;
    }
}