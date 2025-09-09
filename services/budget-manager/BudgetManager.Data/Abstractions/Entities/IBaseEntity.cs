namespace BudgetManager.Data.Abstraction.Entities
{
    public interface IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; }
        public byte[] RowVersion { get; }
        public bool IsDeleted { get; set; }
    }
}
