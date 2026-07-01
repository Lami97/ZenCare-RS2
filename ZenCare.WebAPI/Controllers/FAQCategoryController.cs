using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FAQCategoryController : ControllerBase
{
    private readonly IFAQCategoryService _faqCategoryService;

    public FAQCategoryController(IFAQCategoryService faqCategoryService)
    {
        _faqCategoryService = faqCategoryService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PagedResult<FAQCategoryResponse>>> GetAll([FromQuery] FAQCategorySearchObject? search)
    {
        var result = await _faqCategoryService.GetAllAsync(search);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<FAQCategoryResponse>> GetById(int id)
    {
        var result = await _faqCategoryService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FAQCategoryResponse>> Create([FromBody] FAQCategoryInsertRequest request)
    {
        var result = await _faqCategoryService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FAQCategoryResponse>> Update(int id, [FromBody] FAQCategoryUpdateRequest request)
    {
        var result = await _faqCategoryService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _faqCategoryService.DeleteAsync(id);
        return NoContent();
    }
}
