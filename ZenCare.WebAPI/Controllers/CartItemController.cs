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
public class CartItemController : ControllerBase
{
    private readonly ICartItemService _cartItemService;

    public CartItemController(ICartItemService cartItemService)
    {
        _cartItemService = cartItemService;
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My")]
    public async Task<ActionResult<PagedResult<CartItemResponse>>> GetMy([FromQuery] CartItemSearchObject? search)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My/{id}")]
    public async Task<ActionResult<CartItemResponse>> GetMyById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.GetMyByIdAsync(id, userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<CartItemResponse>>> GetAll([FromQuery] CartItemSearchObject? search)
    {
        var result = await _cartItemService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<CartItemResponse>> GetById(int id)
    {
        var result = await _cartItemService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpPost("My")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartItemResponse>> CreateMy([FromBody] CartItemInsertRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.CreateMyAsync(userId.Value, request);
        return result;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartItemResponse>> Create([FromBody] CartItemInsertRequest request)
    {
        var result = await _cartItemService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = "Client")]
    [HttpPut("My/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartItemResponse>> UpdateMy(int id, [FromBody] CartItemUpdateRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartItemService.UpdateMyAsync(id, userId.Value, request);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartItemResponse>> Update(int id, [FromBody] CartItemUpdateRequest request)
    {
        var result = await _cartItemService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpDelete("My/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMy(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        await _cartItemService.DeleteMyAsync(id, userId.Value);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _cartItemService.DeleteAsync(id);
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}