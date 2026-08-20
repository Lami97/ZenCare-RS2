using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public PurchaseItemController(
        IPurchaseItemService purchaseItemService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _purchaseItemService = purchaseItemService;
        _currentUserAccessor = currentUserAccessor;
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My")]
    public async Task<ActionResult<PagedResult<PurchaseItemResponse>>> GetMy([FromQuery] PurchaseItemSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _purchaseItemService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My/{id}")]
    public async Task<ActionResult<PurchaseItemResponse>> GetMyById(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _purchaseItemService.GetMyByIdAsync(id, userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseItemResponse>>> GetAll([FromQuery] PurchaseItemSearchObject? search)
    {
        var result = await _purchaseItemService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseItemResponse>> GetById(int id)
    {
        var result = await _purchaseItemService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseItemResponse>> Create([FromBody] PurchaseItemInsertRequest request)
    {
        var result = await _purchaseItemService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseItemResponse>> Update(int id, [FromBody] PurchaseItemUpdateRequest request)
    {
        var result = await _purchaseItemService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _purchaseItemService.DeleteAsync(id);
        return NoContent();
    }

}