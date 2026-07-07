using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Responses;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Client,Admin")]
[Route("[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet("Products/{userId}")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetRecommendedProducts(int userId, [FromQuery] int take = 5)
    {
        var result = await _recommendationService.GetRecommendedProductsAsync(userId, take);
        return Ok(result);
    }

    [HttpGet("Services/{userId}")]
    public async Task<ActionResult<List<RecommendationItemResponse>>> GetRecommendedServices(int userId, [FromQuery] int take = 5)
    {
        var result = await _recommendationService.GetRecommendedServicesAsync(userId, take);
        return Ok(result);
    }
}
