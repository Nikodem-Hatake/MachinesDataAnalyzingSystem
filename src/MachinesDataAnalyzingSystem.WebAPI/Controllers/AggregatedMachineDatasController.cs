using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using MachinesDataAnalyzingSystem.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AggregatedMachineDatasController(IAggregatedMachineDatasService aggregatedMachineDatasService,
    IMachinesService machinesService) : ControllerBase
{
    [HttpGet("{machineId:guid}/{startDate:datetime}/{howLong}/{howMany:int}")]
    public async Task<ActionResult<List<AggregatedMachineDataDTO>>> GetAggregatedMachineDatas(Guid machineId, 
        DateTime startDate, HowLongIsAggregatedMachineData howLong, [Range(1, 10)] int howMany)
    {
        return await aggregatedMachineDatasService.GetAggregatedMachineDatas(machineId, startDate, howLong, howMany);
    }

    [HttpGet("{startDate:datetime}/{howLong}/{howMany:int}")]
    public async Task<ActionResult<List<AggregatedMachineDatasForMachineDTO>>> GetAggregatedMachineDatasForAllMachines
        (DateTime startDate, HowLongIsAggregatedMachineData howLong, [Range(1, 7)] int howMany)
    {
        List<AggregatedMachineDatasForMachineDTO> result = [];
        foreach(var machine in await machinesService.GetMachines())
        {
            result.Add(new()
            {
                AggregatedMachineDataDTOS = await aggregatedMachineDatasService.GetAggregatedMachineDatas
                    (machine.MachineId, startDate, howLong, howMany),
                MachineName = machine.MachineName
            });
        }
        return result;
    }
}