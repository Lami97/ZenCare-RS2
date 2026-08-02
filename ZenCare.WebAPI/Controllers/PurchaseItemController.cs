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
public class PurchaseItemController : ControllerBase
{
    private readonly IPurchaseItemService _purchaseItemService;

    public PurchaseItemController(IPurchaseItemService purchaseItemService)
    {
        _purchaseItemService = purchaseItemService;
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My")]
    public async Task<ActionResult<PagedResult<PurchaseItemResponse>>> GetMy([FromQuery] PurchaseItemSearchObject? search)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _purchaseItemService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpGet("My/{id}")]
    public async Task<ActionResult<PurchaseItemResponse>> GetMyById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _purchaseItemService.GetMyByIdAsync(id, userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseItemResponse>>> GetAll([FromQuery] PurchaseItemSearchObject? search)
    {
        var result = await _purchaseItemService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseItemResponse>> GetById(int id)
    {
        var result = await _purchaseItemService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseItemResponse>> Create([FromBody] PurchaseItemInsertRequest request)
    {
        var result = await _purchaseItemService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseItemResponse>> Update(int id, [FromBody] PurchaseItemUpdateRequest request)
    {
        var result = await _purchaseItemService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _purchaseItemService.DeleteAsync(id);
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}