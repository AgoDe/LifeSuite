using System;

namespace BudgetManager.Data.Exceptions
{
    /// <summary>
    /// Eccezione lanciata quando una regola business viene violata
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }

        public BusinessRuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}