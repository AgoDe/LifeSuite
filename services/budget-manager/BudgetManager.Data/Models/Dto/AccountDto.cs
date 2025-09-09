using BudgetManager.Data.Abstraction.Models.Dto;

namespace BudgetManager.Data.Models.Dto
{
    /// <summary>
    /// DTO per la rappresentazione completa di un Account
    /// Include tutti i dati dell'entità per operazioni di lettura
    /// </summary>
    public class AccountDto : IBaseDto
    {
        /// <summary>
        /// Identificatore univoco dell'account
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nome dell'account
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Istituzione bancaria o finanziaria
        /// </summary>
        public string Institution { get; set; } = string.Empty;
        
        /// <summary>
        /// Saldo iniziale dell'account
        /// </summary>
        public decimal InitialBalance { get; set; }
        
        /// <summary>
        /// Saldo corrente dell'account
        /// </summary>
        public decimal Balance { get; set; }
        
        /// <summary>
        /// Data dell'ultimo aggiornamento del saldo
        /// </summary>
        public DateTime BalanceDate { get; set; }
        
        /// <summary>
        /// Data di creazione dell'account
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Versione della riga per controllo concorrenza
        /// </summary>
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    }
}