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
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public AppointmentController(
        IAppointmentService appointmentService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _appointmentService = appointmentService;
        _currentUserAccessor = currentUserAccessor;
    }

    [HttpGet("My")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<PagedResult<AppointmentResponse>>> GetMy([FromQuery] AppointmentSearchObject? search)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _appointmentService.GetMyAsync(userId.Value, search);
        return Ok(result);
    }

    [HttpGet("My/{id}")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<AppointmentResponse>> GetMyById(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _appointmentService.GetMyByIdAsync(id, userId.Value);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("My/available-employees")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<List<AppointmentEmployeeOptionResponse>>> GetAvailableEmployees(
        [FromQuery] int wellnessServiceId,
        [FromQuery] DateTime? appointmentDate,
        [FromQuery] TimeSpan? startTime,
        [FromQuery] TimeSpan? endTime,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        try
        {
            var result = await _appointmentService.GetAvailableEmployeeOptionsAsync(
                wellnessServiceId,
                appointmentDate,
                startTime,
                endTime,
                page,
                pageSize);

            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [Authorize(Roles = AppRoles.AdminOrEmployee)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<AppointmentResponse>>> GetAll([FromQuery] AppointmentSearchObject? search)
    {
        var result = await _appointmentService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.AdminOrEmployee)]
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(int id)
    {
        var result = await _appointmentService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.AdminOrEmployee)]
    [HttpGet("{id}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<AppointmentStatusHistoryResponse>>> GetHistory(
        int id,
        [FromQuery] PagedSearchObject? search)
    {
        var result = await _appointmentService.GetStatusHistoryAsync(id, search);
        return Ok(result);
    }

    [HttpPost("My")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentResponse>> CreateMy([FromBody] AppointmentInsertRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _appointmentService.InsertMyAsync(userId.Value, request);
            return result;
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = AppRoles.AdminOrEmployee)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentResponse>> Create([FromBody] AppointmentInsertRequest request)
    {
        try
        {
            var result = await _appointmentService.InsertAsync(request);
            return result;
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("My/{id}")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> UpdateMy(int id, [FromBody] AppointmentUpdateRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _appointmentService.UpdateMyAsync(id, userId.Value, request);
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

    [HttpPost("My/cancel/{id}")]
    [Authorize(Roles = AppRoles.Client)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> CancelMy(int id, [FromBody] AppointmentCancelRequest request)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _appointmentService.CancelMyAsync(id, userId.Value, request);
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

    [Authorize(Roles = AppRoles.AdminOrEmployee)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> Update(int id, [FromBody] AppointmentUpdateRequest request)
    {
        var actorUserId = _currentUserAccessor.GetUserId();

        if (actorUserId == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _appointmentService.UpdateWithActorAsync(id, actorUserId.Value, request);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
