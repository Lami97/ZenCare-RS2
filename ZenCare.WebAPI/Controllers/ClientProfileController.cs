using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientProfileController : ControllerBase
{
    private readonly IClientProfileService _clientProfileService;

    public ClientProfileController(IClientProfileService clientProfileService)
    {
        _clientProfileService = clientProfileService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ClientProfileResponse>>> GetAll([FromQuery] ClientProfileSearchObject? search)
    {
        var result = await _clientProfileService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientProfileResponse>> GetById(int id)
    {
        var result = await _clientProfileService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientProfileResponse>> Create([FromBody] ClientProfileInsertRequest request)
    {
        var result = await _clientProfileService.InsertAsync(request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientProfileResponse>> Update(int id, [FromBody] ClientProfileUpdateRequest request)
    {
        var result = await _clientProfileService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _clientProfileService.DeleteAsync(id);
        return NoContent();
    }
}
