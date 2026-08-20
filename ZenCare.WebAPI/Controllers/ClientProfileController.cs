using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ClientProfileController(
        IClientProfileService clientProfileService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _clientProfileService = clientProfileService;
        _currentUserAccessor = currentUserAccessor;
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My")]
    public async Task<ActionResult<ClientProfileResponse>> GetMy()
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _clientProfileService.GetMyAsync(userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<ClientProfileResponse>>> GetAll([FromQuery] ClientProfileSearchObject? search)
    {
        var result = await _clientProfileService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientProfileResponse>> GetById(int id)
    {
        var result = await _clientProfileService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientProfileResponse>> Create([FromBody] ClientProfileInsertRequest request)
    {
        var result = await _clientProfileService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpPut("My")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientProfileResponse>> UpdateMy([FromBody] ClientProfileUpdateRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _clientProfileService.UpdateMyAsync(userId.Value, request);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientProfileResponse>> Update(int id, [FromBody] ClientProfileUpdateRequest request)
    {
        var result = await _clientProfileService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _clientProfileService.DeleteAsync(id);
        return NoContent();
    }

}