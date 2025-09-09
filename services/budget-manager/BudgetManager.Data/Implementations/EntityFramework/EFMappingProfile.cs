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
        //CreateMap<UserDM, EFUserEntity>().ReverseMap();
        
        // consigliata da IA
        CreateMap<UserDM, EFUserEntity>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.Accounts, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Recurrings, opt => opt.Ignore());

        CreateMap<EFUserEntity, UserDM>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)) // EF non ha questo campo
            .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => (DateTime?)null)); // EF non ha ques
    }
}