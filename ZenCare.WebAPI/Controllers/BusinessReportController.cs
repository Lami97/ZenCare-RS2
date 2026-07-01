using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BusinessReportController : ControllerBase
{
    private readonly IBusinessReportService _businessReportService;

    public BusinessReportController(IBusinessReportService businessReportService)
    {
        _businessReportService = businessReportService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BusinessReportResponse>>> GetAll([FromQuery] BusinessReportSearchObject? search)
    {
        var result = await _businessReportService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BusinessReportResponse>> GetById(int id)
    {
        var result = await _businessReportService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BusinessReportResponse>> Create([FromBody] BusinessReportInsertRequest request)
    {
        var result = await _businessReportService.InsertAsync(request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusinessReportResponse>> Update(int id, [FromBody] BusinessReportUpdateRequest request)
    {
        var result = await _businessReportService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _businessReportService.DeleteAsync(id);
        return NoContent();
    }
}
