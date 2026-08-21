using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
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
    public async Task<ActionResult<PagedResult<RecommendationItemResponse>>> GetMyRecommendedProducts(
        [FromQuery] RecommendationSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _recommendationService.GetRecommendedProductsAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My/services")]
    public async Task<ActionResult<PagedResult<RecommendationItemResponse>>> GetMyRecommendedServices(
        [FromQuery] RecommendationSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _recommendationService.GetRecommendedServicesAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("Products/{userId}")]
    public async Task<ActionResult<PagedResult<RecommendationItemResponse>>> GetRecommendedProducts(
        int userId,
        [FromQuery] RecommendationSearchObject? search)
    {
        var result = await _recommendationService.GetRecommendedProductsAsync(userId, search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("Services/{userId}")]
    public async Task<ActionResult<PagedResult<RecommendationItemResponse>>> GetRecommendedServices(
        int userId,
        [FromQuery] RecommendationSearchObject? search)
    {
        var result = await _recommendationService.GetRecommendedServicesAsync(userId, search);
        return Ok(result);
    }

}
