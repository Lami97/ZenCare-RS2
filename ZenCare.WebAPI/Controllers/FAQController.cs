using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FAQController : ControllerBase
{
    private readonly IFAQService _faqService;

    public FAQController(IFAQService faqService)
    {
        _faqService = faqService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<FAQResponse>>> GetAll([FromQuery] FAQSearchObject? search)
    {
        var result = await _faqService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FAQResponse>> GetById(int id)
    {
        var result = await _faqService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FAQResponse>> Create([FromBody] FAQInsertRequest request)
    {
        var result = await _faqService.InsertAsync(request);
        return result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FAQResponse>> Update(int id, [FromBody] FAQUpdateRequest request)
    {
        var result = await _faqService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _faqService.DeleteAsync(id);
        return NoContent();
    }
}
