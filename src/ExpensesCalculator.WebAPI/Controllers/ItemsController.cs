using ExpensesCalculator.WebAPI.Filters;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesCalculator.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(IItemService itemService, ILogger<ItemsController> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ValidateResourceAccess(ResourceType.Item)]
    public async Task<ActionResult<ItemDto>> GetItemById(Guid id)
    {
        try
        {
            var result = await _itemService.GetById(id);

            if (result is null)
            {
                _logger.LogWarning("Item {Id} not found", id);
                return NotFound();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpGet("check/{checkId}")]
    [ValidateResourceAccess(ResourceType.Check)]
    public async Task<ActionResult<Item[]>> GetAllCheckItems(Guid checkId)
    {
        try
        {
            var result = await _itemService.GetAllCheckItems(checkId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving items for check {CheckId}", checkId);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpPost]
    [ValidateResourceAccess(ResourceType.Item)]
    public async Task<ActionResult<ItemUpdateResponseDto>> Create([FromBody] CreateItemRequestDto item)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var result = await _itemService.AddItem(item);
            return CreatedAtAction(nameof(GetItemById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Resource not found while creating item: {Message}", ex.Message);
            return NotFound(new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item");
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpPut]
    [ValidateResourceAccess(ResourceType.Item)]
    public async Task<ActionResult<ItemUpdateResponseDto>> Edit([FromBody] EditItemRequestDto item)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var result = await _itemService.EditItem(item);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Item {Id} not found for editing: {Message}", item.Id, ex.Message);
            return NotFound(new { message = "Item not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing item {Id}", item.Id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }

    [HttpDelete("{id}")]
    [ValidateResourceAccess(ResourceType.Item)]
    public async Task<ActionResult<DeleteItemResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _itemService.DeleteItem(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Item {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(new { message = "Item not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item {Id}", id);
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }
}
