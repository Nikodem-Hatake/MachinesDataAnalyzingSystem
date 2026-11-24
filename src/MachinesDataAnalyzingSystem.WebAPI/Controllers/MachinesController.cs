using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared.DTO.Machine;
using MachinesDataAnalyzingSystem.WebAPI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MachinesDataAnalyzingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MachinesController(IMachinesService machinesService) : ControllerBase
{
    [Authorize(Roles = "admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteMachine(MachineDeleteRequest machineDeleteRequest)
    {
        if(await machinesService.DeleteMachine(machineDeleteRequest))
        {
            return Ok();
        }
        return BadRequest("Invalid request.");
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<MachineResponse>>> GetMachines()
    {
        return await machinesService.GetMachines();
    }

    [HttpGet("forService")]
    [ServiceFilter<APIKeyAuthorizationFilter>]
    public async Task<ActionResult<List<MachineResponse>>> GetMachinesForService()
    {
        return await GetMachines();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> PostMachine(MachineAddRequest machineAddRequest)
    {
        if(await machinesService.AddMachine(machineAddRequest))
        {
            return Created();
        }
        return BadRequest("Invalid request.");
    }

    [Authorize(Roles = "admin")]
    [HttpPut]
    public async Task<IActionResult> PutMachine(MachineEditRequest machineEditRequest)
    {
        if(await machinesService.EditMachine(machineEditRequest))
        {
            return Ok();
        }
        return BadRequest("Invalid request");
    }
}