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

    public CategoryController(CategoryService categoryService) : base(categoryService)
    {
        _categoryService = categoryService; 
    }
    
    [HttpGet("select-options/{userId}")]
    public async Task<IActionResult> GetSelectOptions(string userId)
    {
        ApiResponse response;
        try
        {
            var result = await _categoryService.GetSelectOptions(userId);

            response = new ApiResponse<List<SelectOption>>(result);
        }
        catch (Exception ex) 
        {
            response = new ApiResponse(ApiResponse.ApiResponseType.Error);
        }

        return Ok(response);

    }
}