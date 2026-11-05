using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO;
using MachinesDataAnalyzingSystem.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MachinesDataAnalyzingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MachineDatasController(IMachineDatasService machineDataService) : ControllerBase
{
    [HttpPost]
    [ServiceFilter<APIKeyAuthorizationFilter>]
    public async Task PostMachineData(MachineDataDTO machineDataDTO)
    {
        await machineDataService.AddMachineData(machineDataDTO);
    }
}