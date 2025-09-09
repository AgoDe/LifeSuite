using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Models.Dto.Filters
{
    /// <summary>
    /// Filtri per la ricerca e paginazione degli account
    /// </summary>
    public class AccountFilter : UserOwnerListFilter
    {
        /// <summary>
        /// Filtro per nome account (ricerca parziale)
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Filtro per istituzione (ricerca parziale)
        /// </summary>
        public string? Institution { get; set; }
        
        /// <summary>
        /// Filtro per saldo minimo
        /// </summary>
        public decimal? MinBalance { get; set; }
        
        /// <summary>
        /// Filtro per saldo massimo
        /// </summary>
        public decimal? MaxBalance { get; set; }
    }
}