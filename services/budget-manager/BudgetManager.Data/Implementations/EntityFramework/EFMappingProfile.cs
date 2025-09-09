using BudgetManager.Data.Customizations;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Forms;

namespace BudgetManager.Data.Implementations.EntityFramework;

public class EFMappingProfile : MappingProfile
{
    public EFMappingProfile()
    {
        CreateMap<AccountDM, EFAccountEntity>().ReverseMap();
        CreateMap<AccountFormDto, EFAccountEntity>().ReverseMap();
        
        CreateMap<TransactionDM, EFTransactionEntity>().ReverseMap();
        CreateMap<TransactionFormDto, EFTransactionEntity>().ReverseMap();
        
        CreateMap<RecurringDM, EFRecurringEntity>().ReverseMap();
        CreateMap<RecurringFormDto, EFRecurringEntity>().ReverseMap();
        
        CreateMap<CategoryDM, EFCategoryEntity>().ReverseMap();
        
        // Configurazione specifica per gestire il Parent null
        CreateMap<EFCategoryEntity, CategoryDM>()
            .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.ParentId == null ? null : src.Parent));
        CreateMap<CategoryFormDto, EFCategoryEntity>().ReverseMap();
    }
}