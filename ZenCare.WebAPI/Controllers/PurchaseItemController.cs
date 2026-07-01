using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Client,Admin")]
[Route("[controller]")]
public class PurchaseItemController : ControllerBase
{
    private readonly IPurchaseItemService _purchaseItemService;

    public PurchaseItemController(IPurchaseItemService purchaseItemService)
    {
        _purchaseItemService = purchaseItemService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseItemResponse>>> GetAll([FromQuery] PurchaseItemSearchObject? search)
    {
        var result = await _purchaseItemService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseItemResponse>> GetById(int id)
    {
        var result = await _purchaseItemService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseItemResponse>> Create([FromBody] PurchaseItemInsertRequest request)
    {
        var result = await _purchaseItemService.InsertAsync(request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseItemResponse>> Update(int id, [FromBody] PurchaseItemUpdateRequest request)
    {
        var result = await _purchaseItemService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _purchaseItemService.DeleteAsync(id);
        return NoContent();
    }
}
