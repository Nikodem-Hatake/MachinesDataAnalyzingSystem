using MachinesDataAnalyzingSystem.DataAdder;
using MachinesDataAnalyzingSystem.DataAdder.ServiceContracts;
using MachinesDataAnalyzingSystem.DataAdder.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();
builder.Services.AddSingleton<IDataAdder, DataAdderToAPI>();
builder.Services.AddSingleton<IQueueDataReciever, QueueDataReciever>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();