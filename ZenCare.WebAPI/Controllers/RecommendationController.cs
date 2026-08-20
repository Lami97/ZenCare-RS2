using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Responses;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public RecommendationController(
        IRecommendationService recommendationService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _recommendationService = recommendationService;
        _currentUserAccessor = currentUserAccessor;
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My/products")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetMyRecommendedProducts([FromQuery] int take = 5)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _recommendationService.GetRecommendedProductsAsync(userId.Value, take);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My/services")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetMyRecommendedServices([FromQuery] int take = 5)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _recommendationService.GetRecommendedServicesAsync(userId.Value, take);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("Products/{userId}")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetRecommendedProducts(int userId, [FromQuery] int take = 5)
    {
        var result = await _recommendationService.GetRecommendedProductsAsync(userId, take);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("Services/{userId}")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetRecommendedServices(int userId, [FromQuery] int take = 5)
    {
        var result = await _recommendationService.GetRecommendedServicesAsync(userId, take);
        return Ok(result);
    }

}