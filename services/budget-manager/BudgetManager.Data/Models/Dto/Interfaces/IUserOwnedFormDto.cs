using BudgetManager.Data.Abstraction.Models.Dto;

namespace BudgetManager.Data.Models.Dto.Interfaces
{
    /// <summary>
    /// Interfaccia per FormDto che appartengono a un utente specifico
    /// </summary>
    public interface IUserOwnedFormDto : IFormDto
    {
        Guid UserId { get; set; }
    }
}