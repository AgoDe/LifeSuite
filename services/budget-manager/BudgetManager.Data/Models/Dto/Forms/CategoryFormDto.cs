using System.ComponentModel.DataAnnotations;
using BudgetManager.Data.Abstraction.Models.Dto;

namespace BudgetManager.Data.Models.Dto.Forms
{
    public class CategoryFormDto : UserOwnerFormDto
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        public string Name { get; set; } = null!;
        
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        public string Description { get; set; } = string.Empty;
        
        public Guid? ParentId { get; set; }
        
        [Required(ErrorMessage = "Il colore è obbligatorio")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Il colore deve essere in formato esadecimale (es. #FF0000)")]
        public string Color { get; set; } = null!;
    }
}