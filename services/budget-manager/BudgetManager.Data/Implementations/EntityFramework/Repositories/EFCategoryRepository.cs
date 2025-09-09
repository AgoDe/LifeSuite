using AutoMapper;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Data.Implementations.EntityFramework.Repositories
{
    public class EFCategoryRepository : EFCrudRepository<CategoryDM, EFCategoryEntity, CategoryFilter, CategoryFormDto>, ICategoryRepository
    {
        
        private readonly IMapper _mapper;
        public EFCategoryRepository(EFDbcontext db, IMapper mapper) : base(db, mapper)
        {
            _mapper = mapper;
        }

        #region utility
        protected override IQueryable<EFCategoryEntity> GetFilteredQuery(IQueryable<EFCategoryEntity> query, CategoryFilter filters)
        {
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                query = query.Where(
                    q => q.Name.ToLower().Contains(filters.SearchText.ToLower())
                );
            }

            if(filters.UserId != null)
            {
                query = query.Where(c => c.UserId == filters.UserId);
            }

            return query;
        }

        protected override IQueryable<EFCategoryEntity> IncludeProperties(IQueryable<EFCategoryEntity> query)
        {
            return query.Include(x => x.Parent);
        }

        protected override IQueryable<EFCategoryEntity> IncludePropertiesForList(IQueryable<EFCategoryEntity> query)
        {
            return query.Include(x => x.Parent);
        }
        
        protected override IQueryable<EFCategoryEntity> GetOrderedQuery(IQueryable<EFCategoryEntity> query, string orderBy, bool ascending = true)
        {
            switch (orderBy)
            {
                case "name":
                    if (ascending)
                        query = query.OrderBy(q => q.Name);
                    else
                        query = query.OrderByDescending(q => q.Name);
                    break;

                default:
                    if (ascending)
                        query = query.OrderBy(q => q.Name);
                    else
                        query = query.OrderByDescending(q => q.Name);
                    break;
            }

            return query;
        }

        #endregion
    }
}
