import { useApi } from "./useApi"

export const useBudget = () => {
  const { apiCall } = useApi()
  
  // State per i dati budget
  const transactions = ref([])
  const budgetSummary = ref(null)
  const loading = ref(false)
  
  // Ottieni tutte le transazioni
//   const getTransactions = async (filters: any = {}) => {
//     loading.value = true
//     try {
//       const data = await apiCall<any[]>('budget/transactions', {
//         method: 'GET',
//         query: filters
//       })
//       transactions.value = data
//       return data
//     } finally {
//       loading.value = false
//     }
//   }
  
  // Crea nuova transazione
//   const createTransaction = async (transaction: any) => {
//     try {
//       const newTransaction = await apiCall<any>('budget/transactions', {
//         method: 'POST',
//         body: transaction
//       })
      
//       // Aggiorna lo state locale
//       transactions.value.unshift(newTransaction)
      
//       return newTransaction
//     } catch (error) {
//       throw new Error('Failed to create transaction')
//     }
//   }
  
  // Ottieni riassunto budget
//   const getBudgetSummary = async (month?: string) => {
//     try {
//       const summary = await apiCall<any>('budget/summary', {
//         method: 'GET',
//         query: month ? { month } : {}
//       })
//       budgetSummary.value = summary
//       return summary
//     } catch (error) {
//       throw new Error('Failed to get budget summary')
//     }
//   }
  
  return {
    transactions: readonly(transactions),
    budgetSummary: readonly(budgetSummary),
    loading: readonly(loading),
    // getTransactions,
    // createTransaction,
    // getBudgetSummary
  }
}