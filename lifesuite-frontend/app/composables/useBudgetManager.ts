import { useApi } from "./useApi"

export const useBudgetManager = () => {
  const { apiCall } = useApi()
  
  // State for budget data
  const accounts = ref<any[]>([])
  const categories = ref<any[]>([])
  const transactions = ref<any[]>([])
  const recurring = ref<any[]>([])
  const loading = ref(false)
  
  // Accounts API
  const getAccounts = async (filters: any = {}) => {
    loading.value = true
    try {
      const data = await apiCall<any[]>('budget-manager/accounts', {
        method: 'GET',
        query: filters
      })
      accounts.value = data
      return data
    } finally {
      loading.value = false
    }
  }

  const getAccount = async (id: string) => {
    try {
      return await apiCall<any>(`budget-manager/accounts/${id}`, {
        method: 'GET'
      })
    } catch (error) {
      throw new Error('Failed to fetch account')
    }
  }

  const createAccount = async (account: any) => {
    try {
      const newAccount = await apiCall<any>('budget-manager/accounts', {
        method: 'POST',
        body: account
      })
      
      accounts.value.unshift(newAccount)
      return newAccount
    } catch (error) {
      throw new Error('Failed to create account')
    }
  }

  const updateAccount = async (id: string, account: any) => {
    try {
      const updatedAccount = await apiCall<any>(`budget-manager/accounts/${id}`, {
        method: 'PUT',
        body: account
      })
      
      const index = accounts.value.findIndex((a: any) => a.id === id)
      if (index !== -1) {
        accounts.value[index] = updatedAccount
      }
      
      return updatedAccount
    } catch (error) {
      throw new Error('Failed to update account')
    }
  }

  const deleteAccount = async (id: string) => {
    try {
      await apiCall<void>(`budget-manager/accounts/${id}`, {
        method: 'DELETE'
      })
      
      const index = accounts.value.findIndex((a: any) => a.id === id)
      if (index !== -1) {
        accounts.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete account')
    }
  }

  const getAccountSelectOptions = async () => {
    try {
      return await apiCall<any[]>('budget-manager/accounts/select-options', {
        method: 'GET'
      })
    } catch (error) {
      throw new Error('Failed to fetch account options')
    }
  }

  // Categories API
  const getCategories = async (filters: any = {}) => {
    loading.value = true
    try {
      const data = await apiCall<any[]>('budget-manager/categories', {
        method: 'GET',
        query: filters
      })
      categories.value = data
      return data
    } finally {
      loading.value = false
    }
  }

  const createCategory = async (category: any) => {
    try {
      const newCategory = await apiCall<any>('budget-manager/categories', {
        method: 'POST',
        body: category
      })
      
      categories.value.unshift(newCategory)
      return newCategory
    } catch (error) {
      throw new Error('Failed to create category')
    }
  }

  const updateCategory = async (id: string, category: any) => {
    try {
      const updatedCategory = await apiCall<any>(`budget-manager/categories/${id}`, {
        method: 'PUT',
        body: category
      })
      
      const index = categories.value.findIndex((c: any) => c.id === id)
      if (index !== -1) {
        categories.value[index] = updatedCategory
      }
      
      return updatedCategory
    } catch (error) {
      throw new Error('Failed to update category')
    }
  }

  const deleteCategory = async (id: string) => {
    try {
      await apiCall<void>(`budget-manager/categories/${id}`, {
        method: 'DELETE'
      })
      
      const index = categories.value.findIndex((c: any) => c.id === id)
      if (index !== -1) {
        categories.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete category')
    }
  }

  const getCategorySelectOptions = async () => {
    try {
      return await apiCall<any[]>('budget-manager/categories/select-options', {
        method: 'GET'
      })
    } catch (error) {
      throw new Error('Failed to fetch category options')
    }
  }

  // Transactions API
  const getTransactions = async (filters: any = {}) => {
    loading.value = true
    try {
      const data = await apiCall<any[]>('budget-manager/transactions', {
        method: 'GET',
        query: filters
      })
      transactions.value = data
      return data
    } finally {
      loading.value = false
    }
  }

  const getTransactionsWithRecurrings = async (filters: any = {}) => {
    loading.value = true
    try {
      const data = await apiCall<any>('budget-manager/transactions/with-active-recurrings', {
        method: 'GET',
        query: filters
      })
      return data
    } finally {
      loading.value = false
    }
  }

  const createTransaction = async (transaction: any) => {
    try {
      const newTransaction = await apiCall<any>('budget-manager/transactions', {
        method: 'POST',
        body: transaction
      })
      
      transactions.value.unshift(newTransaction)
      return newTransaction
    } catch (error) {
      throw new Error('Failed to create transaction')
    }
  }

  const updateTransaction = async (id: string, transaction: any) => {
    try {
      const updatedTransaction = await apiCall<any>(`budget-manager/transactions/${id}`, {
        method: 'PUT',
        body: transaction
      })
      
      const index = transactions.value.findIndex((t: any) => t.id === id)
      if (index !== -1) {
        transactions.value[index] = updatedTransaction
      }
      
      return updatedTransaction
    } catch (error) {
      throw new Error('Failed to update transaction')
    }
  }

  const deleteTransaction = async (id: string) => {
    try {
      await apiCall<void>(`budget-manager/transactions/${id}`, {
        method: 'DELETE'
      })
      
      const index = transactions.value.findIndex((t: any) => t.id === id)
      if (index !== -1) {
        transactions.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete transaction')
    }
  }

  // Recurring transactions API
  const getRecurring = async (filters: any = {}) => {
    loading.value = true
    try {
      const data = await apiCall<any[]>('budget-manager/recurring', {
        method: 'GET',
        query: filters
      })
      recurring.value = data
      return data
    } finally {
      loading.value = false
    }
  }

  const createRecurring = async (recurringTransaction: any) => {
    try {
      const newRecurring = await apiCall<any>('budget-manager/recurring', {
        method: 'POST',
        body: recurringTransaction
      })
      
      recurring.value.unshift(newRecurring)
      return newRecurring
    } catch (error) {
      throw new Error('Failed to create recurring transaction')
    }
  }

  const updateRecurring = async (id: string, recurringTransaction: any) => {
    try {
      const updatedRecurring = await apiCall<any>(`budget-manager/recurring/${id}`, {
        method: 'PUT',
        body: recurringTransaction
      })
      
      const index = recurring.value.findIndex((r: any) => r.id === id)
      if (index !== -1) {
        recurring.value[index] = updatedRecurring
      }
      
      return updatedRecurring
    } catch (error) {
      throw new Error('Failed to update recurring transaction')
    }
  }

  const deleteRecurring = async (id: string) => {
    try {
      await apiCall<void>(`budget-manager/recurring/${id}`, {
        method: 'DELETE'
      })
      
      const index = recurring.value.findIndex((r: any) => r.id === id)
      if (index !== -1) {
        recurring.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete recurring transaction')
    }
  }

  return {
    // State
    accounts: readonly(accounts),
    categories: readonly(categories),
    transactions: readonly(transactions),
    recurring: readonly(recurring),
    loading: readonly(loading),
    
    // Account methods
    getAccounts,
    getAccount,
    createAccount,
    updateAccount,
    deleteAccount,
    getAccountSelectOptions,
    
    // Category methods
    getCategories,
    createCategory,
    updateCategory,
    deleteCategory,
    getCategorySelectOptions,
    
    // Transaction methods
    getTransactions,
    getTransactionsWithRecurrings,
    createTransaction,
    updateTransaction,
    deleteTransaction,
    
    // Recurring methods
    getRecurring,
    createRecurring,
    updateRecurring,
    deleteRecurring
  }
}