using BudgetManager.Api.Models;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers;

public class CategoryController : CrudController<CategoryDto, CategoryFormDto, CategoryFilter>
{
    
    private readonly CategoryService _categoryService;
    private readonly UserContext _userContext;

    public CategoryController(CategoryService categoryService, UserContext userContext) : base(categoryService)
    {
        _categoryService = categoryService;
        _userContext = userContext;
    }

    [HttpGet("select-options")]
    public async Task<IActionResult> GetSelectOptions()
    {
        ApiResponse response;
        try
        {
            if (string.IsNullOrEmpty(_userContext.UserId))
                return Unauthorized();

            var result = await _categoryService.GetSelectOptions(_userContext.UserId);

            response = new ApiResponse<List<SelectOption>>(result);
        }
        catch (Exception ex) 
        {
            response = new ApiResponse(ApiResponse.ApiResponseType.Error);
        }

        return Ok(response);

    }
}