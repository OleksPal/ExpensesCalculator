using ExpensesCalculator.WebAPI.Filters;
using ExpensesCalculator.WebAPI.Models.Dtos;
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
    private readonly ILogger<ChecksController> _logger;

    public ChecksController(ICheckService checkService, ILogger<ChecksController> logger)
    {
        _checkService = checkService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ValidateResourceAccess(ResourceType.Check)]
    public async Task<ActionResult<CheckDto>> GetCheckById(Guid id)
    {
        try
        {
            var result = await _checkService.GetById(id);

            if (result is null)
            {
                _logger.LogWarning("Check {Id} not found", id);
                return NotFound();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving check {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpGet("day-expenses/{dayExpensesId}")]
    [ValidateResourceAccess(ResourceType.DayExpenses)]
    public async Task<ActionResult<CheckDto[]>> GetAllDayExpensesChecks(Guid dayExpensesId)
    {
        try
        {
            var result = await _checkService.GetAllDayExpensesChecks(dayExpensesId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving checks for day expenses {DayExpensesId}", dayExpensesId);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpPost]
    [ValidateResourceAccess(ResourceType.Check)]
    public async Task<ActionResult<CheckDto>> Create([FromBody] CreateCheckRequestDto check)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var result = await _checkService.AddCheck(check);
            return CreatedAtAction(nameof(GetCheckById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Resource not found while creating check: {Message}", ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating check");
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpPut]
    [ValidateResourceAccess(ResourceType.Check)]
    public async Task<ActionResult<CheckDto>> Edit([FromBody] EditCheckRequestDto check)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var result = await _checkService.UpdateCheck(check);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Check {Id} not found for editing: {Message}", check.Id, ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing check {Id}", check.Id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpDelete("{id}")]
    [ValidateResourceAccess(ResourceType.Check)]
    public async Task<ActionResult<DeleteCheckResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _checkService.DeleteCheck(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Check {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting check {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }
}
