using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZenCare.Model.Responses;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My/products")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetMyRecommendedProducts([FromQuery] int take = 5)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _recommendationService.GetRecommendedProductsAsync(userId.Value, take);
        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My/services")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetMyRecommendedServices([FromQuery] int take = 5)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _recommendationService.GetRecommendedServicesAsync(userId.Value, take);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Products/{userId}")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetRecommendedProducts(int userId, [FromQuery] int take = 5)
    {
        var result = await _recommendationService.GetRecommendedProductsAsync(userId, take);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Services/{userId}")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetRecommendedServices(int userId, [FromQuery] int take = 5)
    {
        var result = await _recommendationService.GetRecommendedServicesAsync(userId, take);
        return Ok(result);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}