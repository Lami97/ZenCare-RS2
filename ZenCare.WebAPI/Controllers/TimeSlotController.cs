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
public class TimeSlotController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<PagedResult<TimeSlotResponse>>> GetAll([FromQuery] TimeSlotSearchObject? search) =>
        Ok(await _timeSlotService.GetAllAsync(search));

    [HttpGet("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<TimeSlotResponse>> GetById(int id) =>
        Ok(await _timeSlotService.GetByIdAsync(id));

    [HttpGet("Available")]
    [Authorize(Roles = AppRoles.Client)]
    public async Task<ActionResult<PagedResult<TimeSlotResponse>>> GetAvailable([FromQuery] TimeSlotSearchObject? search) =>
        Ok(await _timeSlotService.GetAvailableAsync(search));

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<TimeSlotResponse>> Create([FromBody] TimeSlotInsertRequest request)
    {
        try
        {
            return await _timeSlotService.InsertAsync(request);
        }
        catch (BusinessException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<TimeSlotResponse>> Update(int id, [FromBody] TimeSlotUpdateRequest request)
    {
        try
        {
            return Ok(await _timeSlotService.UpdateAsync(id, request));
        }
        catch (BusinessException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _timeSlotService.DeleteAsync(id);
            return NoContent();
        }
        catch (BusinessException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
