using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Client,Admin")]
[Route("[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("My")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<PagedResult<ReviewResponse>>> GetMy([FromQuery] ReviewSearchObject? search)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _reviewService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [HttpGet("My/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<ReviewResponse>> GetMyById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _reviewService.GetMyByIdAsync(id, userId.Value);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ReviewResponse>>> GetAll([FromQuery] ReviewSearchObject? search)
    {
        var result = await _reviewService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetById(int id)
    {
        var result = await _reviewService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReviewResponse>> Create([FromBody] ReviewInsertRequest request)
    {
        var result = await _reviewService.InsertAsync(request);
        return result;
    }

    [HttpPost("My")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReviewResponse>> CreateMy([FromBody] ReviewInsertRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _reviewService.InsertMyAsync(userId.Value, request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponse>> Update(int id, [FromBody] ReviewUpdateRequest request)
    {
        var result = await _reviewService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpPut("My/{id}")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponse>> UpdateMy(int id, [FromBody] ReviewUpdateRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _reviewService.UpdateMyAsync(id, userId.Value, request);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _reviewService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("My/{id}")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMy(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            await _reviewService.DeleteMyAsync(id, userId.Value);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
