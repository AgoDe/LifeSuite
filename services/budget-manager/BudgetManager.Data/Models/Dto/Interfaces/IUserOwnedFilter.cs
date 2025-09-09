namespace BudgetManager.Data.Models.Dto.Interfaces
{
    /// <summary>
    /// Interfaccia per filtri che appartengono a un utente specifico
    /// </summary>
    public interface IUserOwnedFilter : IListFilter
    {
        Guid? UserId { get; set; }
    }
}