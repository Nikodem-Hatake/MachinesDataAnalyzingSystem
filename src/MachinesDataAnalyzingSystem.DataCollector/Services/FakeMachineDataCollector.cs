using MachinesDataAnalyzingSystem.DataCollector.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;
using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;
using System.Text.Json;

namespace MachinesDataAnalyzingSystem.DataCollector.Services;

public class FakeMachineDataCollector : IMachineDataCollector
{
    private List<MachineResponse> _machines = [];
    private readonly Random _random = new();

    public FakeMachineDataCollector(IConfiguration configuration)
    {
        Task.Delay(10000).Wait();    //Waiting for api to be available.
        using var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, configuration.GetConnectionString("WebAPIGetMachineDataEndpoint"));
        request.Headers.Add(GlobalConstants.API_KEY_HEADER_NAME, configuration["APIKey"]);

        _machines = JsonSerializer.Deserialize<List<MachineResponse>>(httpClient.Send(request).Content.ReadAsStringAsync().Result,
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new Exception("Machines are null.");
    }

    public List<MachineDataDTO> CollectNewData()
    {
        List<MachineDataDTO> machinesData = [];

        foreach(var machine in _machines)
        {
            machinesData.Add(GenerateData(machine));
        }

        return machinesData;
    }

    private MachineDataDTO GenerateData(MachineResponse machine)
    {
        var machineData = new MachineDataDTO()
        {
            AverageMotorsSpeed = _random.NextSingle() + (float)_random.Next(10, 15),
            AverageTemperature = _random.NextSingle() + (float)_random.Next(50, 90),
            DateTime = DateTime.UtcNow,
            MachineId = machine.MachineId,
            NumberOfProcessedResources = _random.Next(5, 11),
            NumberOfSecondsToProcessResources = (float)Math.Clamp(_random.NextDouble(), 0.3, 0.5)
        };
        machineData.NumberOfSecondsToProcessResources *= machineData.NumberOfProcessedResources;
        return machineData;
    }
}