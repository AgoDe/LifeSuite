using AutoMapper;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Forms;

namespace BudgetManager.Data.Customizations;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AccountDM, AccountDto>().ReverseMap();
        CreateMap<AccountDM, AccountFormDto>().ReverseMap();

        CreateMap<CategoryDM, CategoryDto>().ReverseMap();
        CreateMap<CategoryDM, CategoryFormDto>().ReverseMap();
        
        CreateMap<TransactionDM, TransactionDto>().ReverseMap();
        CreateMap<TransactionDM, TransactionFormDto>().ReverseMap(); // TODO: ?? non andrà bene
        
        CreateMap<RecurringDM, RecurringDto>().ReverseMap();
        CreateMap<RecurringDM, RecurringFormDto>().ReverseMap(); // TODO: ?? non va bene
    }
}