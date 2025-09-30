import { TransactionType, TransactionStatus, AccountType } from '~/types/budget-manager'

/**
 * Composable per funzioni di utilità del budget manager
 */
export const useBudgetUtils = () => {
  
  // Transaction Type utilities
  const getTransactionTypeColor = (type: TransactionType | string): string => {
    const colors: Record<string, string> = {
      [TransactionType.Income]: 'green',
      [TransactionType.Expense]: 'red',
      'Income': 'green',
      'Expense': 'red',
      'Transfer': 'blue'
    }
    return colors[type] || 'primary'
  }

  const getTransactionTypeIcon = (type: TransactionType | string): string => {
    const icons: Record<string, string> = {
      [TransactionType.Income]: 'mdi-arrow-down',
      [TransactionType.Expense]: 'mdi-arrow-up',
      'Income': 'mdi-arrow-down',
      'Expense': 'mdi-arrow-up',
      'Transfer': 'mdi-swap-horizontal'
    }
    return icons[type] || 'mdi-swap-horizontal'
  }

  const getTransactionTypeLabel = (type: TransactionType | string): string => {
    const labels: Record<string, string> = {
      [TransactionType.Income]: 'Entrata',
      [TransactionType.Expense]: 'Uscita',
      'Income': 'Entrata',
      'Expense': 'Uscita',
      'Transfer': 'Trasferimento'
    }
    return labels[type] || String(type)
  }

  // Transaction Status utilities
  const getTransactionStatusColor = (status: TransactionStatus | string): string => {
    const colors: Record<string, string> = {
      [TransactionStatus.Ignored]: 'grey',
      [TransactionStatus.Estimated]: 'orange',
      [TransactionStatus.Planned]: 'blue',
      [TransactionStatus.Charged]: 'green',
      [TransactionStatus.Cancelled]: 'red',
      'Ignored': 'grey',
      'Estimated': 'orange',
      'Planned': 'blue',
      'Charged': 'green',
      'Cancelled': 'red'
    }
    return colors[status] || 'primary'
  }

  const getTransactionStatusIcon = (status: TransactionStatus | string): string => {
    const icons: Record<string, string> = {
      [TransactionStatus.Ignored]: 'mdi-eye-off',
      [TransactionStatus.Estimated]: 'mdi-calculator',
      [TransactionStatus.Planned]: 'mdi-calendar-clock',
      [TransactionStatus.Charged]: 'mdi-check-circle',
      [TransactionStatus.Cancelled]: 'mdi-cancel',
      'Ignored': 'mdi-eye-off',
      'Estimated': 'mdi-calculator',
      'Planned': 'mdi-calendar-clock',
      'Charged': 'mdi-check-circle',
      'Cancelled': 'mdi-cancel'
    }
    return icons[status] || 'mdi-help-circle'
  }

  const getTransactionStatusLabel = (status: TransactionStatus | string): string => {
    const labels: Record<string, string> = {
      [TransactionStatus.Ignored]: 'Ignorato',
      [TransactionStatus.Estimated]: 'Stimato',
      [TransactionStatus.Planned]: 'Pianificato',
      [TransactionStatus.Charged]: 'Addebitato',
      [TransactionStatus.Cancelled]: 'Annullato',
      'Ignored': 'Ignorato',
      'Estimated': 'Stimato',
      'Planned': 'Pianificato',
      'Charged': 'Addebitato',
      'Cancelled': 'Annullato'
    }
    return labels[status] || String(status)
  }

  // Account Type utilities
  const getAccountTypeIcon = (type: AccountType | string): string => {
    const icons: Record<string, string> = {
      'Checking': 'mdi-bank',
      'Savings': 'mdi-piggy-bank',
      'Credit': 'mdi-credit-card',
      'Investment': 'mdi-trending-up',
      'Cash': 'mdi-cash'
    }
    return icons[String(type)] || 'mdi-bank'
  }

  const getAccountTypeColor = (type: AccountType | string): string => {
    const colors: Record<string, string> = {
      'Checking': 'primary',
      'Savings': 'green',
      'Credit': 'orange',
      'Investment': 'blue',
      'Cash': 'teal'
    }
    return colors[String(type)] || 'primary'
  }

  const getAccountTypeLabel = (type: AccountType | string): string => {
    const labels: Record<string, string> = {
      'Checking': 'Conto Corrente',
      'Savings': 'Conto Risparmio',
      'Credit': 'Carta di Credito',
      'Investment': 'Investimenti',
      'Cash': 'Contanti'
    }
    return labels[String(type)] || String(type)
  }

  // Amount utilities
  const getAmountColor = (type: TransactionType | string, amount: number): string => {
    if (type === TransactionType.Income || type === 'Income') return 'green'
    if (type === TransactionType.Expense || type === 'Expense') return 'red'
    return 'blue'
  }

  // Date utilities
  const formatDate = (date: string): string => {
    return new Date(date).toLocaleDateString('it-IT')
  }

  const formatDateTime = (date: string): string => {
    return new Date(date).toLocaleString('it-IT')
  }

  // Currency utilities
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR'
    }).format(amount)
  }

  // Period utilities
  const getPeriodLabel = (period: string): string => {
    const labels: Record<string, string> = {
      'current_month': 'Mese Corrente',
      'last_month': 'Mese Precedente',
      'next_month': 'Mese Successivo',
      'last_3_months': 'Ultimi 3 Mesi',
      'current_year': 'Anno Corrente',
      'all': 'Tutto'
    }
    return labels[period] || period
  }

  const getMonthLabel = (period: string): string => {
    return getPeriodLabel(period)
  }

  // Select options utilities
  const getTransactionTypeSelectOptions = () => [
    { title: 'Entrata', value: 'Income' },
    { title: 'Uscita', value: 'Expense' },
    { title: 'Trasferimento', value: 'Transfer' }
  ]

  const getTransactionStatusSelectOptions = () => [
    { title: 'Ignorato', value: 'Ignored' },
    { title: 'Stimato', value: 'Estimated' },
    { title: 'Pianificato', value: 'Planned' },
    { title: 'Addebitato', value: 'Charged' },
    { title: 'Annullato', value: 'Cancelled' }
  ]

  const getAccountTypeSelectOptions = () => [
    { title: 'Conto Corrente', value: 'Checking' },
    { title: 'Conto Risparmio', value: 'Savings' },
    { title: 'Carta di Credito', value: 'Credit' },
    { title: 'Investimenti', value: 'Investment' },
    { title: 'Contanti', value: 'Cash' }
  ]

  const getPeriodSelectOptions = () => [
    { title: 'Mese Corrente', value: 'current_month' },
    { title: 'Mese Precedente', value: 'last_month' },
    { title: 'Mese Successivo', value: 'next_month' },
    { title: 'Ultimi 3 Mesi', value: 'last_3_months' },
    { title: 'Anno Corrente', value: 'current_year' },
    { title: 'Tutto', value: 'all' }
  ]

  return {
    // Transaction type
    getTransactionTypeColor,
    getTransactionTypeIcon,
    getTransactionTypeLabel,
    
    // Transaction status
    getTransactionStatusColor,
    getTransactionStatusIcon,
    getTransactionStatusLabel,
    
    // Account type
    getAccountTypeIcon,
    getAccountTypeColor,
    getAccountTypeLabel,
    
    // Amount
    getAmountColor,
    
    // Date
    formatDate,
    formatDateTime,
    
    // Currency
    formatCurrency,
    
    // Period
    getPeriodLabel,
    getMonthLabel,
    
    // Select options
    getTransactionTypeSelectOptions,
    getTransactionStatusSelectOptions,
    getAccountTypeSelectOptions,
    getPeriodSelectOptions
  }
}