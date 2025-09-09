using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Data.Abstraction.Validation
{
    /// <summary>
    /// Rappresenta il risultato di una validazione
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indica se la validazione è stata superata
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// Lista degli errori di validazione
        /// </summary>
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
        
        /// <summary>
        /// Aggiunge un errore di validazione
        /// </summary>
        /// <param name="propertyName">Nome della proprietà che ha generato l'errore</param>
        /// <param name="errorMessage">Messaggio di errore</param>
        public void AddError(string propertyName, string errorMessage)
        {
            Errors.Add(new ValidationError(propertyName, errorMessage));
            IsValid = false;
        }
        
        /// <summary>
        /// Crea un risultato di validazione valido
        /// </summary>
        public static ValidationResult Success() => new ValidationResult { IsValid = true };
        
        /// <summary>
        /// Crea un risultato di validazione con errori
        /// </summary>
        public static ValidationResult Failure(params ValidationError[] errors)
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = errors.ToList()
            };
        }
    }
    
    /// <summary>
    /// Rappresenta un singolo errore di validazione
    /// </summary>
    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        
        public ValidationError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }
}