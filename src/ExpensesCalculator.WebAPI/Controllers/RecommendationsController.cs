using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesCalculator.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly ILogger<RecommendationsController> _logger;

    public RecommendationsController(IRecommendationService recommendationService, ILogger<RecommendationsController> logger)
    {
        _recommendationService = recommendationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<PagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations([FromQuery] AllItemsRequestDto request)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        return await _recommendationService.GetAllItemsForRecommendations(userName, request);
    }

    [HttpGet("tags")]
    public async Task<string[]> GetAllDistinctTags()
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        return await _recommendationService.GetAllDistinctTags(userName);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateRecommendationItemRequestDto item)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            await _recommendationService.AddRecommendationItem(userName, item);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Resource not found while creating recommendation item: {Message}", ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating recommendation item");
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditRecommendationItemRequestDto item)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            await _recommendationService.EditRecommendationItem(userName, item);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Recommendation item {Id} not found for editing: {Message}", item.Id, ex.Message);
            return NotFound(new { message = "Recommendation item not found" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized attempt to edit recommendation item {Id}: {Message}", item.Id, ex.Message);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing recommendation item {Id}", item.Id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            await _recommendationService.DeleteRecommendationItem(userName, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Recommendation item {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(new { message = "Recommendation item not found" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized attempt to delete recommendation item {Id}: {Message}", id, ex.Message);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting recommendation item {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }
}