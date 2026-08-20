using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CartItemController : ControllerBase
{
    private readonly ICartItemService _cartItemService;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public CartItemController(
        ICartItemService cartItemService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _cartItemService = cartItemService;
        _currentUserAccessor = currentUserAccessor;
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My")]
    public async Task<ActionResult<PagedResult<CartItemResponse>>> GetMy([FromQuery] CartItemSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My/{id}")]
    public async Task<ActionResult<CartItemResponse>> GetMyById(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.GetMyByIdAsync(id, userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<CartItemResponse>>> GetAll([FromQuery] CartItemSearchObject? search)
    {
        var result = await _cartItemService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<CartItemResponse>> GetById(int id)
    {
        var result = await _cartItemService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpPost("My")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartItemResponse>> CreateMy([FromBody] CartItemInsertRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.CreateMyAsync(userId.Value, request);
        return result;
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartItemResponse>> Create([FromBody] CartItemInsertRequest request)
    {
        var result = await _cartItemService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpPut("My/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartItemResponse>> UpdateMy(int id, [FromBody] CartItemUpdateRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.UpdateMyAsync(id, userId.Value, request);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartItemResponse>> Update(int id, [FromBody] CartItemUpdateRequest request)
    {
        var result = await _cartItemService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpDelete("My/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMy(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        await _cartItemService.DeleteMyAsync(id, userId.Value);
        return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _cartItemService.DeleteAsync(id);
        return NoContent();
    }

}