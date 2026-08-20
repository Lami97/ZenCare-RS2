using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ProductController(
        IProductService productService,
        ICurrentUserAccessor currentUserAccessor)
    {
        _productService = productService;
        _currentUserAccessor = currentUserAccessor;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetAll([FromQuery] ProductSearchObject? search)
    {
        var result = await _productService.GetAllAsync(search);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Client)]
    [HttpPost("My/{id}/view")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordMyView(int id)
    {
        var userId = _currentUserAccessor.GetUserId();

        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        await _productService.RecordMyViewAsync(id, userId.Value);
        return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] ProductInsertRequest request)
    {
        var result = await _productService.InsertAsync(request);
        return result;
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update(int id, [FromBody] ProductUpdateRequest request)
    {
        var result = await _productService.UpdateAsync(id, request);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
