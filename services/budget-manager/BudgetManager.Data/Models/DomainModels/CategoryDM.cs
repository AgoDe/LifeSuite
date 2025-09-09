namespace BudgetManager.Data.Models.DomainModels;

public class CategoryDM : BaseDomainModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    
    // Navigational properties
    public virtual CategoryDM? Parent { get; set; }
    public virtual ICollection<CategoryDM> Children { get; set; } = new List<CategoryDM>();
}