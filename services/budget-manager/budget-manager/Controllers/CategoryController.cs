using System.Security.Claims;
using BudgetManager.Api.Extensions;
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
    
    [HttpGet("select-options")]
    public async Task<IActionResult> GetSelectOptions()
    {
        ApiResponse response;
        try
        {
            var userId = HttpContext.GetCurrentUserId();
            if(userId == null)
                return Unauthorized();
            
            var result = await _categoryService.GetSelectOptions(userId.Value);

            response = new ApiResponse<List<SelectOption>>(result);
        }
        catch (Exception ex) 
        {
            response = new ApiResponse(ApiResponse.ApiResponseType.Error);
        }

        return Ok(response);

    }
}