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
    private readonly ILogger<DayExpensesController> _logger;

    public DayExpensesController(IDayExpensesService dayExpensesService, ILogger<DayExpensesController> logger)
    {
        _dayExpensesService = dayExpensesService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultWithDateRangeDto<DayExpensesResponseDto>>> GetAllDays([FromQuery] AllDayExpensesRequestDto request)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        return await _dayExpensesService.GetAllDays(userName, request);
    }

    [Route("{id}/details")]
    [HttpGet]
    public async Task<ActionResult<DayExpensesDetailsResponseDto>> GetDayExpensesDetailsById(Guid id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var result = await _dayExpensesService.GetDayExpensesDetails(id, userName);

        if (result is null)
        {
            _logger.LogWarning("Day expenses {Id} not found for user {UserName}", id, userName);
            return NotFound();
        }

        return result;
    }

    [Route("{id}/calculate")]
    [HttpGet]
    public async Task<ActionResult<DayExpensesCalculationsDto>> CalculateExpenses(Guid id)
    {
        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return await _dayExpensesService.GetDayExpensesCalculations(id, userName);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Day expenses {Id} not found for calculation: {Message}", id, ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating expenses for day {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<DayExpensesResponseDto>> Create([FromBody] CreateDayExpensesRequestDto dayExpenses)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _dayExpensesService.AddDayExpenses(dayExpenses, userName);
            return CreatedAtAction(nameof(GetDayExpensesDetailsById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error creating day expenses for user {UserName}: {Message}", User.FindFirstValue(ClaimTypes.Name), ex.Message);
            ModelState.AddModelError("Participants", "Invalid participants list");
            return ValidationProblem(ModelState);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Resource not found while creating day expenses: {Message}", ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating day expenses for user {UserName}", User.FindFirstValue(ClaimTypes.Name));
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpPut]
    public async Task<ActionResult<DayExpensesResponseDto>> Edit([FromBody] EditDayExpensesRequestDto dayExpenses)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _dayExpensesService.EditDayExpenses(dayExpenses, userName);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error editing day expenses {Id}: {Message}", dayExpenses.Id, ex.Message);
            ModelState.AddModelError("Participants", "Invalid participants list");
            return ValidationProblem(ModelState);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Day expenses {Id} not found for editing: {Message}", dayExpenses.Id, ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing day expenses {Id}", dayExpenses.Id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            await _dayExpensesService.DeleteDayExpenses(id, userName);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Day expenses {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting day expenses {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }        
    }

    [HttpPost("{id}/share")]
    public async Task<ActionResult<DayExpensesResponseDto>> Share(Guid id, ShareDayExpensesRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _dayExpensesService.ShareDayExpenses(id, request.NewUserWithAccess, userName);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to share day expenses {Id} with user {NewUser}: {Error}", id, request.NewUserWithAccess, result.Error);
            }

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Day expenses {Id} not found for sharing: {Message}", id, ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing day expenses {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }
}
