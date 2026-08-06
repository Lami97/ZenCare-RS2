using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpGet("My")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<PagedResult<PurchaseResponse>>> GetMy([FromQuery] PurchaseSearchObject? search)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _purchaseService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [HttpGet("My/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<PurchaseResponse>> GetMyById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _purchaseService.GetMyByIdAsync(id, userId.Value);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseResponse>>> GetAll([FromQuery] PurchaseSearchObject? search)
    {
        var result = await _purchaseService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseResponse>> GetById(int id)
    {
        var result = await _purchaseService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseResponse>> Create([FromBody] PurchaseInsertRequest request)
    {
        try
        {
            var result = await _purchaseService.InsertAsync(request);
            return result;
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("My")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseResponse>> CreateMy([FromBody] PurchaseInsertRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _purchaseService.InsertMyAsync(userId.Value, request);
            return result;
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Checkout")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseResponse>> Checkout([FromBody] PurchaseCheckoutRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _purchaseService.CheckoutAsync(userId.Value, request);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("My/cancel/{id}")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> CancelMy(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _purchaseService.CancelMyAsync(id, userId.Value);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> Update(int id, [FromBody] PurchaseUpdateRequest request)
    {
        try
        {
            var result = await _purchaseService.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("My/{id}")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> UpdateMy(int id, [FromBody] PurchaseUpdateRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _purchaseService.UpdateMyAsync(id, userId.Value, request);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _purchaseService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("My/{id}")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMy(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            await _purchaseService.DeleteMyAsync(id, userId.Value);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
