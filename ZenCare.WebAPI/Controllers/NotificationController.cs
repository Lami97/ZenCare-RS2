using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My")]
    public async Task<ActionResult<PagedResult<NotificationResponse>>> GetMy([FromQuery] NotificationSearchObject? search)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _notificationService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My/{id}")]
    public async Task<ActionResult<NotificationResponse>> GetMyById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _notificationService.GetMyByIdAsync(id, userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<NotificationResponse>>> GetAll([FromQuery] NotificationSearchObject? search)
    {
        var result = await _notificationService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationResponse>> GetById(int id)
    {
        var result = await _notificationService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationResponse>> Create([FromBody] NotificationInsertRequest request)
    {
        var result = await _notificationService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationResponse>> Update(int id, [FromBody] NotificationUpdateRequest request)
    {
        var result = await _notificationService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _notificationService.DeleteAsync(id);
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}