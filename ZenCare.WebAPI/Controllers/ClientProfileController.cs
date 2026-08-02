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
public class ClientProfileController : ControllerBase
{
    private readonly IClientProfileService _clientProfileService;

    public ClientProfileController(IClientProfileService clientProfileService)
    {
        _clientProfileService = clientProfileService;
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My")]
    public async Task<ActionResult<ClientProfileResponse>> GetMy()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _clientProfileService.GetMyAsync(userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<ClientProfileResponse>>> GetAll([FromQuery] ClientProfileSearchObject? search)
    {
        var result = await _clientProfileService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientProfileResponse>> GetById(int id)
    {
        var result = await _clientProfileService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientProfileResponse>> Create([FromBody] ClientProfileInsertRequest request)
    {
        var result = await _clientProfileService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = "Client")]
    [HttpPut("My")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientProfileResponse>> UpdateMy([FromBody] ClientProfileUpdateRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _clientProfileService.UpdateMyAsync(userId.Value, request);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientProfileResponse>> Update(int id, [FromBody] ClientProfileUpdateRequest request)
    {
        var result = await _clientProfileService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _clientProfileService.DeleteAsync(id);
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}