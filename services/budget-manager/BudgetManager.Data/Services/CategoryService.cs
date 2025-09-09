using AutoMapper;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;

namespace BudgetManager.Data.Services;
public class CategoryService : BaseCrudService<CategoryDM, CategoryDto, CategoryFormDto, CategoryFilter>
{
    private readonly IRepositoryUnitOfWork _repositoryUOW;
    private IMapper _mapper;
    
    public CategoryService(
        IMapper mapper, 
        IRepositoryUnitOfWork repositoryUow
    ) : base(repositoryUow, mapper)
    {
        _mapper = mapper;
        _repositoryUOW = repositoryUow;
    }
    
    public async Task<List<SelectOption>> GetSelectOptions(string userId)
    {
        var categories = await _repository.GetListAsync(new CategoryFilter { UserId = userId });

        return categories.Items
            .Select(c => new SelectOption()
            {
                Title = c.Name,
                Value = c.Id.ToString(),
                ParentTitle = c.Parent?.Name,
                ParentValue = c.Parent != null ? c.Parent.Id.ToString() : null
            })
            .OrderBy(opt => opt.Title)
            .OrderBy(opt => opt.ParentTitle)
            .ToList();
    
    }
    
}