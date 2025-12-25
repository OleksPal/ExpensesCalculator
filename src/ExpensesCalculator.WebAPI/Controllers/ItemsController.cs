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

    [HttpGet]
    public async Task<Item> GetItemById(Guid id)
    {
        return await _itemService.GetById(id);
    }

    [HttpPost]
    public async Task Create(Item item)
    {
        await _itemService.AddItem(item);
    }

    [HttpPut]
    public async Task Edit([FromBody] Item item)
    {
        await _itemService.EditItem(item);
    }

    [HttpDelete]  
    public async Task DeleteConfirmed(Guid id)
    {
        await _itemService.DeleteItem(id);
    }
}
