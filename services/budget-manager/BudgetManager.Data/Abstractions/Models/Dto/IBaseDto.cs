using System;

namespace BudgetManager.Data.Abstraction.Models.Dto
{
    /// <summary>
    /// Interfaccia base per tutti i DTO
    /// Rappresenta la struttura minima per il trasferimento dati
    /// </summary>
    public interface IBaseDto
    {
        /// <summary>
        /// Identificatore univoco dell'entità
        /// </summary>
        Guid Id { get; set; }
        
        /// <summary>
        /// Data di creazione dell'entità
        /// </summary>
        DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Versione della riga per controllo concorrenza
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}