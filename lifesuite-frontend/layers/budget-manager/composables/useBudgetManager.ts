import { useApi } from "../../../app/composables/useApi"
import type { 
  AccountDto, 
  CategoryDto, 
  TransactionDto, 
  RecurringDto,
  TransactionFilter,
  AccountFilter,
  CategoryFilter,
  RecurringFilter,
  TransactionFormDto,
  AccountFormDto,
  CategoryFormDto,
  RecurringFormDto,
  SelectOption,
  TransactionListItem
} from "~/types/budget-manager"

export const useBudgetManager = () => {
  const { apiCall } = useApi()
  
  // State for budget data
  const accounts = ref<AccountDto[]>([])
  const categories = ref<CategoryDto[]>([])
  const transactions = ref<TransactionDto[]>([])
  const recurring = ref<RecurringDto[]>([])
  const loading = ref(false)
  
  // Accounts API
  const getAccounts = async (filters: AccountFilter = {}) => {
    loading.value = true
    try {
      const data = await apiCall<AccountDto[]>('budget-manager/accounts', {
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
      return await apiCall<AccountDto>(`budget-manager/accounts/${id}`, {
        method: 'GET'
      })
    } catch (error) {
      throw new Error('Failed to fetch account')
    }
  }

  const createAccount = async (account: AccountFormDto) => {
    try {
      const newAccount = await apiCall<AccountDto>('budget-manager/accounts', {
        method: 'POST',
        body: account
      })
      
      accounts.value.unshift(newAccount)
      return newAccount
    } catch (error) {
      throw new Error('Failed to create account')
    }
  }

  const updateAccount = async (id: string, account: AccountFormDto) => {
    try {
      const updatedAccount = await apiCall<AccountDto>(`budget-manager/accounts/${id}`, {
        method: 'PUT',
        body: account
      })
      
      const index = accounts.value.findIndex((a: AccountDto) => a.id === id)
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
      
      const index = accounts.value.findIndex((a: AccountDto) => a.id === id)
      if (index !== -1) {
        accounts.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete account')
    }
  }

  const getAccountSelectOptions = async () => {
    try {
      return await apiCall<SelectOption[]>('budget-manager/accounts/select-options', {
        method: 'GET'
      })
    } catch (error) {
      throw new Error('Failed to fetch account options')
    }
  }

  // Categories API
  const getCategories = async (filters: CategoryFilter = {}) => {
    loading.value = true
    try {
      const data = await apiCall<CategoryDto[]>('budget-manager/categories', {
        method: 'GET',
        query: filters
      })
      categories.value = data
      return data
    } finally {
      loading.value = false
    }
  }

  const createCategory = async (category: CategoryFormDto) => {
    try {
      const newCategory = await apiCall<CategoryDto>('budget-manager/categories', {
        method: 'POST',
        body: category
      })
      
      categories.value.unshift(newCategory)
      return newCategory
    } catch (error) {
      throw new Error('Failed to create category')
    }
  }

  const updateCategory = async (id: string, category: CategoryFormDto) => {
    try {
      const updatedCategory = await apiCall<CategoryDto>(`budget-manager/categories/${id}`, {
        method: 'PUT',
        body: category
      })
      
      const index = categories.value.findIndex((c: CategoryDto) => c.id === id)
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
      
      const index = categories.value.findIndex((c: CategoryDto) => c.id === id)
      if (index !== -1) {
        categories.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete category')
    }
  }

  const getCategorySelectOptions = async () => {
    try {
      return await apiCall<SelectOption[]>('budget-manager/categories/select-options', {
        method: 'GET'
      })
    } catch (error) {
      throw new Error('Failed to fetch category options')
    }
  }

  // Transactions API
  const getTransactions = async (filters: TransactionFilter = {}) => {
    loading.value = true
    try {
      const data = await apiCall<TransactionDto[]>('budget-manager/transactions', {
        method: 'GET',
        query: filters
      })
      transactions.value = data
      return data
    } finally {
      loading.value = false
    }
  }

  const getTransactionsWithRecurrings = async (filters: TransactionFilter = {}) => {
    loading.value = true
    try {
      const data = await apiCall<TransactionDto[]>('budget-manager/transactions/with-active-recurrings', {
        method: 'GET',
        query: filters
      })
      return data
    } finally {
      loading.value = false
    }
  }

  const createTransaction = async (transaction: TransactionFormDto) => {
    try {
      const newTransaction = await apiCall<TransactionDto>('budget-manager/transactions', {
        method: 'POST',
        body: transaction
      })
      
      transactions.value.unshift(newTransaction)
      return newTransaction
    } catch (error) {
      throw new Error('Failed to create transaction')
    }
  }

  const updateTransaction = async (id: string, transaction: TransactionFormDto) => {
    try {
      const updatedTransaction = await apiCall<TransactionDto>(`budget-manager/transactions/${id}`, {
        method: 'PUT',
        body: transaction
      })
      
      const index = transactions.value.findIndex((t: TransactionDto) => t.id === id)
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
      
      const index = transactions.value.findIndex((t: TransactionDto) => t.id === id)
      if (index !== -1) {
        transactions.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete transaction')
    }
  }

  // Recurring transactions API
  const getRecurring = async (filters: RecurringFilter = {}) => {
    loading.value = true
    try {
      const data = await apiCall<RecurringDto[]>('budget-manager/recurring', {
        method: 'GET',
        query: filters
      })
      recurring.value = data
      return data
    } finally {
      loading.value = false
    }
  }

  const createRecurring = async (recurringTransaction: RecurringFormDto) => {
    try {
      const newRecurring = await apiCall<RecurringDto>('budget-manager/recurring', {
        method: 'POST',
        body: recurringTransaction
      })
      
      recurring.value.unshift(newRecurring)
      return newRecurring
    } catch (error) {
      throw new Error('Failed to create recurring transaction')
    }
  }

  const updateRecurring = async (id: string, recurringTransaction: RecurringFormDto) => {
    try {
      const updatedRecurring = await apiCall<RecurringDto>(`budget-manager/recurring/${id}`, {
        method: 'PUT',
        body: recurringTransaction
      })
      
      const index = recurring.value.findIndex((r: RecurringDto) => r.id === id)
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
      
      const index = recurring.value.findIndex((r: RecurringDto) => r.id === id)
      if (index !== -1) {
        recurring.value.splice(index, 1)
      }
    } catch (error) {
      throw new Error('Failed to delete recurring transaction')
    }
  }

  // Utility method to combine transactions and recurring as unified list
  const getCombinedTransactionList = async (filters: TransactionFilter = {}): Promise<TransactionListItem[]> => {
    try {
      loading.value = true
      
      // Get regular transactions
      const transactionsData = await getTransactions(filters)
      
      // Get active recurring transactions for the same account/period
      const recurringData = await getRecurring({ 
        accountId: filters.accountId,
        isActive: true 
      })
      
      const combined: TransactionListItem[] = []
      
      // Add regular transactions
      transactionsData.forEach(t => {
        combined.push({
          id: t.id,
          description: t.description,
          date: t.date,
          type: t.type,
          amount: t.amount,
          status: t.status,
          notes: t.notes,
          category: t.category,
          isRecurring: false,
          isEstimated: t.isEstimated || false
        })
      })
      
      // Add recurring transactions as estimated future transactions
      recurringData.forEach(r => {
        // Calculate next occurrence(s) within the filter period
        const today = new Date()
        const activeFrom = new Date(r.activeFrom)
        const activeTo = new Date(r.activeTo)
        
        if (today >= activeFrom && today <= activeTo) {
          // Generate next occurrence
          const nextDate = new Date(today.getFullYear(), today.getMonth(), r.chargeDay)
          if (nextDate < today) {
            nextDate.setMonth(nextDate.getMonth() + 1)
          }
          
          if (nextDate <= activeTo) {
            combined.push({
              id: `recurring-${r.id}-${nextDate.getTime()}`,
              description: `${r.description} (Ricorrente)`,
              date: nextDate.toISOString(),
              type: r.type,
              amount: r.amount,
              status: r.status || 2, // Estimated
              notes: r.notes,
              category: r.category,
              isRecurring: true,
              isEstimated: true,
              recurringInfo: {
                activeFrom: r.activeFrom,
                activeTo: r.activeTo,
                chargeDay: r.chargeDay,
                isActive: r.isActive || true
              }
            })
          }
        }
      })
      
      // Sort by date
      combined.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
      
      return combined
      
    } finally {
      loading.value = false
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
    deleteRecurring,
    
    // Combined methods
    getCombinedTransactionList
  }
}