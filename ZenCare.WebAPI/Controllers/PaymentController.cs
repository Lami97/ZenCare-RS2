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
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public PaymentController(
        IPaymentService paymentService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _paymentService = paymentService;
        _currentUserAccessor = currentUserAccessor;
    }

    [HttpPost("My/create-intent/{purchaseId}")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentIntentResponse>> CreatePaymentIntent(int purchaseId)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _paymentService.CreatePaymentIntentAsync(purchaseId, userId.Value);
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

    [HttpPost("My/confirm/{purchaseId}")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentConfirmResponse>> ConfirmPayment(int purchaseId)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _paymentService.ConfirmPaymentAsync(purchaseId, userId.Value);
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

    [HttpPost("My/refund/{purchaseId}")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentRefundResponse>> RefundPayment(int purchaseId)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _paymentService.RefundPaymentAsync(purchaseId, userId.Value);
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

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My")]
    public async Task<ActionResult<PagedResult<PaymentResponse>>> GetMy([FromQuery] PaymentSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _paymentService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpGet("My/{id}")]
    public async Task<ActionResult<PaymentResponse>> GetMyById(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _paymentService.GetMyByIdAsync(id, userId.Value);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<PaymentResponse>>> GetAll([FromQuery] PaymentSearchObject? search)
    {
        var result = await _paymentService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentResponse>> GetById(int id)
    {
        var result = await _paymentService.GetByIdAsync(id);
        return Ok(result);
    }

}
