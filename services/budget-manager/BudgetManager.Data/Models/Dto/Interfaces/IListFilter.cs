namespace BudgetManager.Data.Models.Dto.Interfaces
{
    /// <summary>
    /// Interfaccia base per tutti i filtri di lista
    /// Fornisce proprietà comuni per paginazione e ordinamento
    /// </summary>
    public interface IListFilter
    {
        /// <summary>
        /// Numero di pagina (base 1)
        /// </summary>
        int PageNumber { get; set; }
        
        /// <summary>
        /// Dimensione della pagina
        /// </summary>
        int PageSize { get; set; }
        
        /// <summary>
        /// Campo per ordinamento
        /// </summary>
        string? OrderBy { get; set; }
        
        /// <summary>
        /// Direzione ordinamento (true = ascendente, false = discendente)
        /// </summary>
        bool Ascending { get; set; }
        
        string? SearchText { get; set; }
    }
}