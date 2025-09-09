using BudgetManager.Data.Abstraction.Models.Dto;

namespace BudgetManager.Data.Models.Dto
{
    /// <summary>
    /// Classe base per tutti i DTO
    /// Rappresenta la struttura minima per il trasferimento dati
    /// </summary>
    public class BaseDto : IBaseDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] RowVersion { get; set; } = null!;
    }
}