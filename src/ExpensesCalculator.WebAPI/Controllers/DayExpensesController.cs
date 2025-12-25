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
    public async Task<DayExpenses> GetDayExpensesById(Guid id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        return await _dayExpensesService.GetById(id, userName);
    }

    /// TODO: Fix error handling
    /// OPTIONAL: Add Notes field for dayExpenses/checks

    //[HttpGet]
    //public async Task<IActionResult> CalculateExpenses(int id)
    //{
    //    if (User.Identity.Name is not null)
    //        _dayExpensesService.RequestorName = User.Identity.Name;

    //    var dayExpensesCalculation = await _dayExpensesService.GetCalculationForDayExpenses(id);

    //    if (dayExpensesCalculation is null)
    //        return NotFound();

    //    return View(dayExpensesCalculation);
    //}

    [HttpPost]
    public async Task Create([FromBody] CreateDayExpensesRequestDto dayExpenses)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        await _dayExpensesService.AddDayExpenses(dayExpenses, userName);
    }

    [HttpPut]
    public async Task Edit([FromBody] EditDayExpensesRequestDto dayExpenses)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        await _dayExpensesService.EditDayExpenses(dayExpenses, userName);
    }

    [HttpDelete("{id}")]
    public async Task Delete(Guid id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        await _dayExpensesService.DeleteDayExpenses(id, userName);
    }

    [HttpPost("{id}/share")]
    public async Task<IActionResult> Share(Guid id, ShareDayExpensesRequestDto request)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var result = await _dayExpensesService.ShareDayExpenses(id, request.NewUserWithAccess, userName);
        return Ok(result);
    }
}
