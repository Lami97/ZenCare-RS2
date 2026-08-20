using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("User/My/profile")]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ProfileController(
        IUserService userService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _userService = userService;
        _currentUserAccessor = currentUserAccessor;
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserResponse>> GetMyProfile()
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _userService.GetMyProfileAsync(userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserResponse>> UpdateMyProfile([FromBody] UpdateMyProfileRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _userService.UpdateMyProfileAsync(userId.Value, request);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpPut("/User/My/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ChangePasswordResponse>> ChangeMyPassword([FromBody] ChangePasswordRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _userService.ChangeMyPasswordAsync(userId.Value, request);
        return Ok(result);
    }

}
