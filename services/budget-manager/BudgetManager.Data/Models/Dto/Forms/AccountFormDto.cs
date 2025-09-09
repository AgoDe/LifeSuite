using System.ComponentModel.DataAnnotations;
using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Models.Dto.Forms
{
    /// <summary>
    /// DTO per form di creazione e modifica Account
    /// Contiene solo i dati modificabili dall'utente
    /// </summary>
    public class AccountFormDto : UserOwnerFormDto, IFormDto
    {

        /// <summary>
        /// Nome dell'account
        /// </summary>
        [Required(ErrorMessage = "Il nome dell'account è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Istituzione bancaria o finanziaria
        /// </summary>
        [StringLength(100, ErrorMessage = "Il nome dell'istituzione non può superare i 100 caratteri")]
        public string Institution { get; set; } = string.Empty;
        
        /// <summary>
        /// Saldo iniziale dell'account
        /// </summary>
        [Range(-999999999.99, 999999999.99, ErrorMessage = "Il saldo deve essere compreso tra -999,999,999.99 e 999,999,999.99")]
        public decimal Balance { get; set; }

    }
}