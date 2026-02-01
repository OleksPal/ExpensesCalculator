using ExpensesCalculator.WebAPI.Models;
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

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }
    // TODO: Rename Description column to Comment
    [HttpGet]
    public async Task<Item> GetItemById(Guid id)
    {
        return await _itemService.GetById(id);
    }

    [HttpGet("check/{checkId}")]
    public async Task<ICollection<Item>> GetAllCheckItems(Guid checkId)
    {
        return await _itemService.GetAllCheckItems(checkId);
    }

    [HttpPost]
    public async Task<decimal> Create([FromBody] Item item)
    {
        return await _itemService.AddItem(item);
    }

    [HttpPut]
    public async Task<decimal> Edit([FromBody] Item item)
    {
        return await _itemService.EditItem(item);
    }

    [HttpDelete]  
    public async Task<decimal> Delete(Guid id)
    {
        return await _itemService.DeleteItem(id);
    }
}
