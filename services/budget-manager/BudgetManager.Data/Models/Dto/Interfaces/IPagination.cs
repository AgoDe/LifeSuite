using System.Text.Json.Serialization;

namespace BudgetManager.Data.Models.Dto.Interfaces;

public class IPagination
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }
    
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
    
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
    
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}