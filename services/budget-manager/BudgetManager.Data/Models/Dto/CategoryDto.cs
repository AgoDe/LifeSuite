using BudgetManager.Data.Abstraction.Models.Dto;

namespace BudgetManager.Data.Models.Dto
{
    public class CategoryDto : IBaseDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public CategoryDto? Parent { get; set; }
        public string Color { get; set; } = null!;
        
    }
}