using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ReviewController(
        IReviewService reviewService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _reviewService = reviewService;
        _currentUserAccessor = currentUserAccessor;
    }

    [HttpGet("My")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<PagedResult<ReviewResponse>>> GetMy([FromQuery] ReviewSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _reviewService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [HttpGet("My/{id}")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<ReviewResponse>> GetMyById(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

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

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<ReviewResponse>>> GetAll([FromQuery] ReviewSearchObject? search)
    {
        var result = await _reviewService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetById(int id)
    {
        var result = await _reviewService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReviewResponse>> Create([FromBody] ReviewInsertRequest request)
    {
        var result = await _reviewService.InsertAsync(request);
        return result;
    }

    [HttpPost("My")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReviewResponse>> CreateMy([FromBody] ReviewInsertRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _reviewService.InsertMyAsync(userId.Value, request);
            return result;
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = AppRoles.Admin)]
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
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponse>> UpdateMy(int id, [FromBody] ReviewUpdateRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

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
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _reviewService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("My/{id}")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMy(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

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

}
