using System.Collections.Generic;
using BudgetManager.Data.Models.Dto.Interfaces;
using System.Text.Json.Serialization;

namespace BudgetManager.Data.Models.Dto
{
    /// <summary>
    /// Rappresenta una lista paginata di elementi
    /// </summary>
    /// <typeparam name="T">Tipo degli elementi nella lista</typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// Elementi della pagina corrente
        /// </summary>
        [JsonPropertyName("items")]
        public ICollection<T> Items { get; set; } = new List<T>();
        
        [JsonPropertyName("pagination")]
        public IPagination? Pagination { get; set; }
        
        /// <summary>
        /// Numero totale di elementi (tutte le pagine)
        /// </summary>
       /*  [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Numero di pagina corrente (base 1)
        /// </summary>
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }
        
        /// <summary>
        /// Dimensione della pagina
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        
        /// <summary>
        /// Numero totale di pagine
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        
        /// <summary>
        /// Indica se esiste una pagina precedente
        /// </summary>
        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => PageNumber > 1;
        
        /// <summary>
        /// Indica se esiste una pagina successiva
        /// </summary>
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => PageNumber < TotalPages; */
        
        public PaginatedList(ICollection<T> items)
        {
            Items = items;
        }

        public PaginatedList(ICollection<T> items, IPagination pagination)
        {
            Items = items;
            Pagination = pagination;
        }

        public PaginatedList(List<T> items, int pageNumber, int pageSize, int count)
        {
            Items = items;
            Pagination = new Pagination(pageNumber, pageSize, count);
        }
    }
}