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
    public async Task DeleteMachine(MachineDeleteRequest machineDeleteRequest)
    {
        await machinesService.DeleteMachine(machineDeleteRequest);
    }

    [Authorize]
    [HttpGet]
    public async Task<List<MachineResponse>> GetMachines()
    {
        return await machinesService.GetMachines();
    }

    [HttpGet("forService")]
    [ServiceFilter<APIKeyAuthorizationFilter>]
    public async Task<List<MachineResponse>> GetMachinesForService()
    {
        return await GetMachines();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task PostMachine(MachineAddRequest machineAddRequest)
    {
        await machinesService.AddMachine(machineAddRequest);
    }

    [Authorize(Roles = "admin")]
    [HttpPut]
    public async Task PutMachine(MachineEditRequest machineEditRequest)
    {
        await machinesService.EditMachine(machineEditRequest);
    }
}