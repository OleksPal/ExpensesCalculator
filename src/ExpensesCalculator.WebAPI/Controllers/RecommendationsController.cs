using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesCalculator.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public RecommendationsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<RecommendationsPagedResultDto<RecommendationItemDto>> GetAllAvailableItems([FromQuery] AllDayExpensesRequestDto request)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return await _itemService.GetAllItemsForRecommendations(userName, request);
        }

        [HttpGet("tags")]
        public async Task<ICollection<string>> GetAllDistinctTags()
        {
            return await _itemService.GetAllDistinctTags();
        }
    }
}
