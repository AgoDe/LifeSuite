namespace BudgetManager.Data.Models.Dto.Filters
{
    public class CategoryFilter : UserOwnerListFilter
    {
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }
        public string? Color { get; set; }
    }
}