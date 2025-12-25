using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesCalculator.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChecksController : ControllerBase
{
    private readonly ICheckService _checkService;

    public ChecksController(ICheckService checkService)
    {
        _checkService = checkService;
    }

    [HttpGet]
    public async Task<Check> GetCheckById(Guid id)
    {
        return await _checkService.GetById(id);
    }

    [HttpPost]
    public async Task CreateCheck([FromBody] Check check)
    {
        await _checkService.AddCheck(check);
    }

    [HttpPut]
    public async Task Edit([FromBody] Check check)
    {
        await _checkService.UpdateCheck(check);
    }

    [HttpDelete]
    public async Task DeleteConfirmed(Guid id)
    {
        await _checkService.DeleteCheck(id);
    }
}
