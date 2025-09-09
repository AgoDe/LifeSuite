namespace BudgetManager.Data.Models.Enums
{
    /// <summary>
    /// Tipo di transazione per operazioni sui conti
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Entrata - aumenta il saldo
        /// </summary>
        Income = 1,
        
        /// <summary>
        /// Uscita - diminuisce il saldo
        /// </summary>
        Expense = 2,
    }
}