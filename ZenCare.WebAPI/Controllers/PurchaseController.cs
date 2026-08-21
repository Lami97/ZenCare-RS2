using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public PurchaseController(
        IPurchaseService purchaseService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _purchaseService = purchaseService;
        _currentUserAccessor = currentUserAccessor;
    }

    [HttpGet("My")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<PagedResult<PurchaseResponse>>> GetMy([FromQuery] PurchaseSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _purchaseService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [HttpGet("My/{id}")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<PurchaseResponse>> GetMyById(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

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

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseResponse>>> GetAll([FromQuery] PurchaseSearchObject? search)
    {
        var result = await _purchaseService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseResponse>> GetById(int id)
    {
        var result = await _purchaseService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<PurchaseStatusHistoryResponse>>> GetHistory(
        int id,
        [FromQuery] PagedSearchObject? search)
    {
        var result = await _purchaseService.GetStatusHistoryAsync(id, search);
        return Ok(result);
    }

    [HttpPost("Checkout")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseResponse>> Checkout([FromBody] PurchaseCheckoutRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

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
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> CancelMy(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

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

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseResponse>> Update(int id, [FromBody] PurchaseUpdateRequest request)
    {
        var actorUserId = _currentUserAccessor.GetUserId();

        if (actorUserId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _purchaseService.UpdateWithActorAsync(id, actorUserId.Value, request);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
