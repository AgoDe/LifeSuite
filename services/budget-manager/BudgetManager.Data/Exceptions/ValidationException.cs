using BudgetManager.Data.Abstraction.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Data.Exceptions
{
    /// <summary>
    /// Eccezione lanciata quando la validazione di un DTO fallisce
    /// </summary>
    public class ValidationException : Exception
    {
        public IEnumerable<ValidationError> Errors { get; }

        public ValidationException(IEnumerable<ValidationError> errors) 
            : base($"Validation failed: {string.Join(", ", errors.Select(e => e.ErrorMessage))}")
        {
            Errors = errors;
        }

        public ValidationException(ValidationError error) 
            : this(new[] { error })
        {
        }

        public ValidationException(string field, string message) 
            : this(new ValidationError(field, message))
        {
        }
    }
}