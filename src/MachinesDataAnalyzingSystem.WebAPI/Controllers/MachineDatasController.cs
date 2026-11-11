using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;
using MachinesDataAnalyzingSystem.WebAPI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MachinesDataAnalyzingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MachineDatasController(IMachineDatasService machineDataService) : ControllerBase
{
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<ActionResult<MachineDatasCountResponse>> GetMachineDatasCount()
    {
        return Ok(await machineDataService.GetMachineDatasCount());
    }

    [HttpPost]
    [ServiceFilter<APIKeyAuthorizationFilter>]
    public async Task PostMachineData(MachineDataDTO machineDataDTO)
    {
        await machineDataService.AddMachineData(machineDataDTO);
    }
}