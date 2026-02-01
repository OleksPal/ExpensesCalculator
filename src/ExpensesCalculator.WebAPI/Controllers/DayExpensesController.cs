using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesCalculator.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DayExpensesController : ControllerBase
{
    private readonly IDayExpensesService _dayExpensesService;

    public DayExpensesController(IDayExpensesService dayExpensesService)
    {
        _dayExpensesService = dayExpensesService;
    }

    [HttpGet]
    public async Task<PagedResultWithDateRangeDto<DayExpensesResponseDto>> GetAllDays([FromQuery] AllDayExpensesRequestDto request)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        return await _dayExpensesService.GetAllDays(userName, request);
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<DayExpensesResponseDto> GetDayExpensesById(Guid id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        return await _dayExpensesService.GetById(id, userName);
    }

    [Route("{id}/calculate")]
    [HttpGet]
    public async Task<IActionResult> CalculateExpenses(Guid id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var dayExpensesCalculation = await _dayExpensesService.GetCalculationForDayExpenses(id, userName);

        if (dayExpensesCalculation is null)
            return NotFound();

        return Ok(dayExpensesCalculation);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDayExpensesRequestDto dayExpenses)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var insertedDayId = await _dayExpensesService.AddDayExpenses(dayExpenses, userName);
            return Ok(insertedDayId);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("Participants", ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] EditDayExpensesRequestDto dayExpenses)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            await _dayExpensesService.EditDayExpenses(dayExpenses, userName);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("Participants", ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        await _dayExpensesService.DeleteDayExpenses(id, userName);
        return Ok();
    }

    [HttpPost("{id}/share")]
    public async Task<IActionResult> Share(Guid id, ShareDayExpensesRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userName = User.FindFirstValue(ClaimTypes.Name);
        var result = await _dayExpensesService.ShareDayExpenses(id, request.NewUserWithAccess, userName);
        return Ok(result);
    }
}
