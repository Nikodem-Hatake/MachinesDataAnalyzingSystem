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
        return await machineDataService.GetMachineDatasCount();
    }

    [HttpPost]
    [ServiceFilter<APIKeyAuthorizationFilter>]
    public async Task<IActionResult> PostMachineData(MachineDataDTO machineDataDTO)
    {
        if(await machineDataService.AddMachineData(machineDataDTO))
        {
            return Created();
        }
        return BadRequest($"Machine with Id {machineDataDTO.MachineId} was not found.");
    }
}