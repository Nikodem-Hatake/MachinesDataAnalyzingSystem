using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;

namespace MachinesDataAnalyzingSystem.Core.Services;

public class MachineDatasService(IMachineDatasRepository machineDatasRepository, 
    IMachinesRepository machinesRepository) : IMachineDatasService
{
    public async Task<bool> AddMachineData(MachineDataDTO machineDataDTO)
    {
        var machineData = new MachineData()
        {
            AverageMotorsSpeed = machineDataDTO.AverageMotorsSpeed,
            AverageTemperature = machineDataDTO.AverageTemperature,
            DateTime = machineDataDTO.DateTime,
            Id = Guid.NewGuid(),
            MachineId = machineDataDTO.MachineId,
            NumberOfProcessedResources = machineDataDTO.NumberOfProcessedResources,
            NumberOfSecondsToProcessResources = machineDataDTO.NumberOfSecondsToProcessResources
        };
        return await machineDatasRepository.AddMachineData(machineData);
    }

    public async Task<MachineDatasCountResponse> GetMachineDatasCount()
    {
        return new()
        {
            Count = await machineDatasRepository.GetMachineDatasCount()
        };
    }
}