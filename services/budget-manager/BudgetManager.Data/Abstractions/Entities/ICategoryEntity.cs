namespace BudgetManager.Data.Abstraction.Entities
{
    public interface ICategoryEntity : IBaseEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        Guid? ParentId { get; set; }
        string Color { get; set; }
        int UserId { get; set; }
    }
}
