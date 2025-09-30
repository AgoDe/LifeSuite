// Base interfaces
export interface IBaseDto {
  id: string
  createdAt: string
  rowVersion: string
}

export interface IFormDto {
  userId?: string
}

// Enums
export enum TransactionType {
  Income = 1,
  Expense = 2
}

export enum TransactionStatus {
  Ignored = 1,
  Estimated = 2,
  Planned = 3,
  Charged = 4,
  Cancelled = 5
}

export enum AccountType {
  Checking = 'Checking',
  Savings = 'Savings',
  Credit = 'Credit',
  Investment = 'Investment',
  Cash = 'Cash'
}

// Main DTOs
export interface AccountDto extends IBaseDto {
  name: string
  institution: string
  initialBalance: number
  balance: number
  balanceDate: string
  type?: AccountType
  description?: string
}

export interface CategoryDto extends IBaseDto {
  name: string
  description: string
  parent?: CategoryDto | null
  color: string
  icon?: string
}

export interface TransactionDto extends IBaseDto {
  description: string
  date: string
  type: TransactionType
  amount: number
  status: TransactionStatus
  notes?: string | null
  account: AccountDto
  category: CategoryDto
  recurring?: RecurringDto | null
  relatedTransaction?: TransactionDto | null
  isEstimated?: boolean
}

export interface RecurringDto extends IBaseDto {
  description: string
  institution?: string | null
  notes?: string | null
  activeFrom: string
  activeTo: string
  chargeDay: number
  amount: number
  type: TransactionType
  userId: string
  account: AccountDto
  category: CategoryDto
  relatedRecurring?: RecurringDto | null
  isActive?: boolean
  status?: TransactionStatus
}

// Form DTOs
export interface TransactionFormDto extends IFormDto {
  description: string
  date: string
  type: TransactionType | string
  amount: number
  categoryId: string
  notes?: string | null
  accountId: string
  recurringId?: string | null
  relatedAccountId?: string | null
  status: TransactionStatus
}

export interface AccountFormDto extends IFormDto {
  name: string
  institution: string
  initialBalance: number
  type: AccountType | string
  description?: string
}

export interface CategoryFormDto extends IFormDto {
  name: string
  description: string
  parentId?: string | null
  color: string
  icon?: string
}

export interface RecurringFormDto extends IFormDto {
  description: string
  institution?: string | null
  notes?: string | null
  activeFrom: string
  activeTo: string
  chargeDay: number
  amount: number
  type: TransactionType | string
  accountId: string
  categoryId: string
  relatedRecurringId?: string | null
}

// Filter DTOs
export interface TransactionFilter {
  dateFrom?: string | null
  dateTo?: string | null
  amountFrom?: number | null
  amountTo?: number | null
  amountMin?: number | null
  amountMax?: number | null
  type?: TransactionType | string | null
  categoryId?: string | null
  categoryIds?: string[] | null
  accountId?: string | null
  accountIds?: string[] | null
  status?: TransactionStatus | null
  statusFrom?: TransactionStatus | null
  statusTo?: TransactionStatus | null
  includeTransfers?: boolean
  includeRecurrings?: boolean
  description?: string | null
}

export interface AccountFilter {
  name?: string | null
  institution?: string | null
  type?: AccountType | string | null
  balanceFrom?: number | null
  balanceTo?: number | null
}

export interface CategoryFilter {
  name?: string | null
  parentId?: string | null
  color?: string | null
}

export interface RecurringFilter {
  description?: string | null
  activeFromDate?: string | null
  activeToDate?: string | null
  type?: TransactionType | string | null
  accountId?: string | null
  categoryId?: string | null
  isActive?: boolean | null
}

// Select Options
export interface SelectOption {
  value: string
  title: string
  text?: string
  disabled?: boolean
}

// Pagination
export interface PaginatedList<T> {
  items: T[]
  totalCount: number
  pageSize: number
  currentPage: number
  totalPages: number
  hasNext: boolean
  hasPrevious: boolean
}

// Combined transaction and recurring display item
export interface TransactionListItem {
  id: string
  description: string
  date: string
  type: TransactionType | string
  amount: number
  status: TransactionStatus | number
  notes?: string | null
  category: CategoryDto
  isRecurring: boolean
  isEstimated?: boolean
  recurringInfo?: {
    activeFrom: string
    activeTo: string
    chargeDay: number
    isActive: boolean
  }
}