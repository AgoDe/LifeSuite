namespace BudgetManager.Data.Models.Dto;

public class SelectOption
{
    public string Title { get; set; }
    public string Value { get; set; }

    public string? ParentTitle { get; set; }

    public string? ParentValue { get; set; }

}