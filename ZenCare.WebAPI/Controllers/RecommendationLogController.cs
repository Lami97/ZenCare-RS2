using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class RecommendationLogController : ControllerBase
{
    private readonly IRecommendationLogService _recommendationLogService;

    public RecommendationLogController(IRecommendationLogService recommendationLogService)
    {
        _recommendationLogService = recommendationLogService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<RecommendationLogResponse>>> GetAll([FromQuery] RecommendationLogSearchObject? search)
    {
        var result = await _recommendationLogService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecommendationLogResponse>> GetById(int id)
    {
        var result = await _recommendationLogService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecommendationLogResponse>> Create([FromBody] RecommendationLogInsertRequest request)
    {
        var result = await _recommendationLogService.InsertAsync(request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecommendationLogResponse>> Update(int id, [FromBody] RecommendationLogUpdateRequest request)
    {
        var result = await _recommendationLogService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _recommendationLogService.DeleteAsync(id);
        return NoContent();
    }
}
