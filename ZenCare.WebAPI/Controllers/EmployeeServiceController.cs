using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Employee,Admin")]
[Route("[controller]")]
public class EmployeeServiceController : ControllerBase
{
    private readonly IEmployeeServiceService _employeeServiceService;

    public EmployeeServiceController(IEmployeeServiceService employeeServiceService)
    {
        _employeeServiceService = employeeServiceService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeServiceResponse>>> GetAll([FromQuery] EmployeeServiceSearchObject? search)
    {
        var result = await _employeeServiceService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeServiceResponse>> GetById(int id)
    {
        var result = await _employeeServiceService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeServiceResponse>> Create([FromBody] EmployeeServiceInsertRequest request)
    {
        var result = await _employeeServiceService.InsertAsync(request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeServiceResponse>> Update(int id, [FromBody] EmployeeServiceUpdateRequest request)
    {
        var result = await _employeeServiceService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeServiceService.DeleteAsync(id);
        return NoContent();
    }
}
