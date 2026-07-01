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
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserRoleResponse>>> GetAll([FromQuery] UserRoleSearchObject? search)
    {
        var result = await _userRoleService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserRoleResponse>> GetById(int id)
    {
        var result = await _userRoleService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleResponse>> Create([FromBody] UserRoleInsertRequest request)
    {
        var result = await _userRoleService.InsertAsync(request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserRoleResponse>> Update(int id, [FromBody] UserRoleUpdateRequest request)
    {
        var result = await _userRoleService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _userRoleService.DeleteAsync(id);
        return NoContent();
    }
}
