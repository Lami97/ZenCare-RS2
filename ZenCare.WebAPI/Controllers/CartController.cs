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
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My")]
    public async Task<ActionResult<CartResponse>> GetMy()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartService.GetMyAsync(userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<CartResponse>>> GetAll([FromQuery] CartSearchObject? search)
    {
        var result = await _cartService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<CartResponse>> GetById(int id)
    {
        var result = await _cartService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpPost("My")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartResponse>> CreateMy([FromBody] CartInsertRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartService.CreateMyAsync(userId.Value, request);
        return result;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartResponse>> Create([FromBody] CartInsertRequest request)
    {
        var result = await _cartService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = "Client")]
    [HttpPut("My/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartResponse>> UpdateMy(int id, [FromBody] CartUpdateRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _cartService.UpdateMyAsync(id, userId.Value, request);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> Update(int id, [FromBody] CartUpdateRequest request)
    {
        var result = await _cartService.UpdateAsync(id, request);
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

        await _cartService.DeleteMyAsync(id, userId.Value);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _cartService.DeleteAsync(id);
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}