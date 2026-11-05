using MachinesDataAnalyzingSystem.DataCollector;
using MachinesDataAnalyzingSystem.DataCollector.ServiceContracts;
using MachinesDataAnalyzingSystem.DataCollector.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();
builder.Services.AddSingleton<IQueueDataAdder, QueueDataAdder>();
builder.Services.AddSingleton<IMachineDataCollector, FakeMachineDataCollector>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();