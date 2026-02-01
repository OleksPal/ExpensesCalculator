using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Services;
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

    [Route("day-expenses/{dayExpensesId}")]
    [HttpGet]
    public async Task<ICollection<CheckDto>> GetAllDayExpensesChecks(Guid dayExpensesId)
    {
        return await _checkService.GetAllDayExpensesChecks(dayExpensesId);
    }

    [HttpGet]
    public async Task<CheckDto> GetCheckById(Guid id)
    {
        return await _checkService.GetById(id);
    }

    [HttpPost]
    public async Task Create([FromBody] Check check)
    {
        await _checkService.AddCheck(check);
    }

    [HttpPut]
    public async Task Edit([FromBody] Check check)
    {
        await _checkService.UpdateCheck(check);
    }

    [HttpDelete("{id}")]
    public async Task Delete(Guid id)
    {
        await _checkService.DeleteCheck(id);
    }
}
