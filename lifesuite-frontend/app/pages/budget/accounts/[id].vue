<template>
  <div>
    <!-- Loading State -->
    <v-skeleton-loader
      v-if="loading"
      type="article"
      class="mx-auto"
    />

    <div v-else>
      <!-- Account Header -->
      <v-row class="mb-4">
        <v-col>
          <v-btn
            variant="text"
            prepend-icon="mdi-arrow-left"
            class="mb-3"
            @click="$router.back()"
          >
            Torna agli Account
          </v-btn>
          
          <div class="d-flex align-center mb-2">
            <v-icon
              :color="getAccountTypeColor(account?.type || '')"
              size="32"
              class="mr-3"
            >
              {{ getAccountTypeIcon(account?.type || '') }}
            </v-icon>
            <div>
              <h2 class="text-h4">{{ account?.name }}</h2>
              <p class="text-subtitle-1 text-medium-emphasis">
                {{ account?.description }}
              </p>
            </div>
          </div>
        </v-col>
        <v-col cols="auto" class="text-right">
          <div class="text-h3 mb-1">
            {{ formatCurrency(account?.balance || 0) }}
          </div>
          <v-chip
            :color="getAccountTypeColor(account?.type || '')"
            variant="tonal"
          >
            {{ account?.type }}
          </v-chip>
        </v-col>
      </v-row>

      <!-- Quick Filters Row -->
      <v-row class="mb-4">
        <v-col cols="12" md="6">
          <v-select
            v-model="selectedPeriod"
            :items="periodOptions"
            label="Periodo"
            prepend-icon="mdi-calendar"
            @update:model-value="onPeriodChange"
          />
        </v-col>
        <v-col cols="12" md="6" class="d-flex align-center justify-end gap-2">
          <v-btn
            color="secondary"
            prepend-icon="mdi-filter"
            @click="advancedFiltersDialog = true"
          >
            Filtri Avanzati
          </v-btn>
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            @click="openTransactionDialog()"
          >
            Nuova Transazione
          </v-btn>
        </v-col>
      </v-row>

      <!-- Statistics Section -->
      <v-row class="mb-4">
        <v-col cols="12" md="4">
          <v-card>
            <v-card-title class="d-flex align-center">
              <v-icon color="green" class="mr-2">mdi-arrow-down</v-icon>
              Entrate
            </v-card-title>
            <v-card-text>
              <div class="text-h5 text-green">
                {{ formatCurrency(statistics.totalIncome) }}
              </div>
              <div class="text-caption text-medium-emphasis">
                {{ statistics.incomeCount }} transazioni
              </div>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" md="4">
          <v-card>
            <v-card-title class="d-flex align-center">
              <v-icon color="red" class="mr-2">mdi-arrow-up</v-icon>
              Uscite
            </v-card-title>
            <v-card-text>
              <div class="text-h5 text-red">
                {{ formatCurrency(statistics.totalExpenses) }}
              </div>
              <div class="text-caption text-medium-emphasis">
                {{ statistics.expenseCount }} transazioni
              </div>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" md="4">
          <v-card>
            <v-card-title class="d-flex align-center">
              <v-icon color="blue" class="mr-2">mdi-chart-line</v-icon>
              Saldo Netto
            </v-card-title>
            <v-card-text>
              <div class="text-h5" :class="statistics.netAmount >= 0 ? 'text-green' : 'text-red'">
                {{ formatCurrency(statistics.netAmount) }}
              </div>
              <div class="text-caption text-medium-emphasis">
                Differenza entrate/uscite
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Forecast Section -->
      <v-card class="mb-4" v-if="forecast.length > 0">
        <v-card-title>
          <v-icon class="mr-2">mdi-crystal-ball</v-icon>
          Previsioni per {{ getMonthLabel(selectedPeriod) }}
        </v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Entrate Previste</div>
              <div class="text-h6 text-green">{{ formatCurrency(forecastStats.expectedIncome) }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Uscite Previste</div>
              <div class="text-h6 text-red">{{ formatCurrency(forecastStats.expectedExpenses) }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Saldo Previsto</div>
              <div class="text-h6" :class="forecastStats.expectedNet >= 0 ? 'text-green' : 'text-red'">
                {{ formatCurrency(forecastStats.expectedNet) }}
              </div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!-- Transactions Table -->
      <v-card>
        <v-card-title>
          Transazioni e Ricorrenze
          <v-spacer />
          <span class="text-caption">{{ transactionList.length }} elementi</span>
        </v-card-title>
        
        <v-data-table
          :headers="headers"
          :items="transactionList"
          :loading="transactionsLoading"
          class="elevation-0"
          items-per-page="15"
        >
          <template v-slot:item.date="{ item }">
            {{ formatDate(item.date) }}
          </template>

          <template v-slot:item.amount="{ item }">
            <v-chip
              :color="getAmountColor(item.type, item.amount)"
              size="small"
              variant="tonal"
            >
              {{ formatCurrency(item.amount) }}
            </v-chip>
          </template>

          <template v-slot:item.type="{ item }">
            <div class="d-flex align-center">
              <v-icon
                :color="getTransactionTypeColor(item.type)"
                size="small"
                class="mr-1"
              >
                {{ getTransactionTypeIcon(item.type) }}
              </v-icon>
              {{ getTransactionTypeLabel(item.type) }}
            </div>
          </template>

          <template v-slot:item.status="{ item }">
            <v-chip
              :color="getTransactionStatusColor(item.status)"
              size="small"
              variant="tonal"
            >
              <v-icon size="small" class="mr-1">
                {{ getTransactionStatusIcon(item.status) }}
              </v-icon>
              {{ getTransactionStatusLabel(item.status) }}
            </v-chip>
          </template>

          <template v-slot:item.category="{ item }">
            <div v-if="item.category" class="d-flex align-center">
              <v-icon 
                :color="item.category.color || 'primary'" 
                size="small" 
                class="mr-1"
              >
                {{ item.category.icon || 'mdi-tag' }}
              </v-icon>
              {{ item.category.name }}
            </div>
            <span v-else class="text-medium-emphasis">Nessuna categoria</span>
          </template>

          <template v-slot:item.isRecurring="{ item }">
            <v-chip
              v-if="item.isRecurring"
              color="purple"
              size="small"
              variant="tonal"
            >
              <v-icon size="small" class="mr-1">mdi-repeat</v-icon>
              Ricorrente
            </v-chip>
            <v-chip
              v-else-if="item.isEstimated"
              color="orange"
              size="small"
              variant="tonal"
            >
              <v-icon size="small" class="mr-1">mdi-calculator</v-icon>
              Stimato
            </v-chip>
            <span v-else class="text-medium-emphasis">Normale</span>
          </template>

          <template v-slot:item.actions="{ item }">
            <v-btn
              v-if="!item.isRecurring"
              variant="text"
              color="secondary"
              icon="mdi-pencil"
              size="small"
              @click="openTransactionDialog(item)"
            />
            <v-btn
              v-if="!item.isRecurring"
              variant="text"
              color="error"
              icon="mdi-delete"
              size="small"
              @click="confirmDeleteTransaction(item)"
            />
            <v-tooltip v-else text="Gestisci nella sezione Ricorrenze">
              <template v-slot:activator="{ props }">
                <v-btn
                  v-bind="props"
                  variant="text"
                  color="purple"
                  icon="mdi-open-in-new"
                  size="small"
                  @click="$router.push('/budget/recurring')"
                />
              </template>
            </v-tooltip>
          </template>

          <template v-slot:no-data>
            <div class="text-center pa-8">
              <v-icon size="64" color="grey-lighten-2" class="mb-4">
                mdi-swap-horizontal
              </v-icon>
              <h3 class="text-h6 mb-2">Nessuna transazione nel periodo selezionato</h3>
              <p class="text-body-2 text-medium-emphasis mb-4">
                Prova a cambiare il periodo o registra una nuova transazione
              </p>
            </div>
          </template>
        </v-data-table>
      </v-card>
    </div>

    <!-- Advanced Filters Dialog -->
    <v-dialog v-model="advancedFiltersDialog" max-width="500px">
      <v-card>
        <v-card-title>Filtri Avanzati</v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="advancedFilters.dateFrom"
                label="Data Da"
                type="date"
              />
            </v-col>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="advancedFilters.dateTo"
                label="Data A"
                type="date"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="advancedFilters.amountMin"
                label="Importo Minimo"
                type="number"
                step="0.01"
                prefix="€"
              />
            </v-col>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="advancedFilters.amountMax"
                label="Importo Massimo"
                type="number"
                step="0.01"
                prefix="€"
              />
            </v-col>
          </v-row>
          <v-select
            v-model="advancedFilters.type"
            :items="transactionTypes"
            label="Tipo Transazione"
            clearable
          />
          <v-text-field
            v-model="advancedFilters.description"
            label="Cerca nella descrizione"
            prepend-icon="mdi-magnify"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn @click="clearAdvancedFilters">Reset</v-btn>
          <v-spacer />
          <v-btn @click="advancedFiltersDialog = false">Annulla</v-btn>
          <v-btn color="primary" @click="applyAdvancedFilters">Applica</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Transaction Dialog -->
    <v-dialog v-model="transactionDialog" max-width="600px">
      <v-card>
        <v-card-title>
          {{ editingTransaction ? 'Modifica Transazione' : 'Nuova Transazione' }}
        </v-card-title>

        <v-card-text>
          <v-form ref="transactionFormRef" v-model="formValid">
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="transactionForm.description"
                  label="Descrizione"
                  :rules="[v => !!v || 'Descrizione obbligatoria']"
                  required
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="transactionForm.amount"
                  label="Importo"
                  type="number"
                  step="0.01"
                  prefix="€"
                  :rules="[v => v > 0 || 'Importo deve essere maggiore di 0']"
                  required
                />
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <v-select
                  v-model="transactionForm.type"
                  :items="transactionTypes"
                  label="Tipo Transazione"
                  :rules="[v => !!v || 'Tipo obbligatorio']"
                  required
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="transactionForm.date"
                  label="Data"
                  type="date"
                  :rules="[v => !!v || 'Data obbligatoria']"
                  required
                />
              </v-col>
            </v-row>

            <v-select
              v-model="transactionForm.categoryId"
              :items="categoryOptions"
              label="Categoria"
            />

            <v-textarea
              v-model="transactionForm.notes"
              label="Note"
              rows="2"
            />
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn @click="closeTransactionDialog">Annulla</v-btn>
          <v-btn
            color="primary"
            :loading="submitting"
            @click="saveTransaction"
          >
            {{ editingTransaction ? 'Aggiorna' : 'Registra' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400px">
      <v-card>
        <v-card-title>Conferma Eliminazione</v-card-title>
        <v-card-text>
          Sei sicuro di voler eliminare la transazione "{{ transactionToDelete?.description }}"?
          Questa azione non può essere annullata.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="deleteDialog = false">Annulla</v-btn>
          <v-btn
            color="error"
            :loading="deleting"
            @click="deleteTransactionHandler"
          >
            Elimina
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Snackbar for notifications -->
    <v-snackbar
      v-model="snackbar.show"
      :color="snackbar.color"
      :timeout="3000"
    >
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import type { 
  AccountDto, 
  TransactionListItem, 
  TransactionFilter, 
  TransactionFormDto,
  SelectOption
} from '~/types/budget-manager'

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

const route = useRoute()
const accountId = route.params.id as string

const { 
  getAccount,
  getCombinedTransactionList,
  createTransaction,
  updateTransaction,
  deleteTransaction: deleteTransactionApi,
  getCategorySelectOptions
} = useBudgetManager()

const {
  formatDate,
  formatCurrency,
  getAmountColor,
  getTransactionTypeColor,
  getTransactionTypeIcon,
  getTransactionTypeLabel,
  getTransactionStatusColor,
  getTransactionStatusIcon,
  getTransactionStatusLabel,
  getAccountTypeIcon,
  getAccountTypeColor,
  getPeriodLabel,
  getTransactionTypeSelectOptions
} = useBudgetUtils()

// Reactive state
const account = ref<AccountDto | null>(null)
const transactionList = ref<TransactionListItem[]>([])
const categoryOptions = ref<SelectOption[]>([])
const loading = ref(false)
const transactionsLoading = ref(false)
const transactionDialog = ref(false)
const advancedFiltersDialog = ref(false)
const deleteDialog = ref(false)
const editingTransaction = ref<TransactionListItem | null>(null)
const transactionToDelete = ref<TransactionListItem | null>(null)
const submitting = ref(false)
const deleting = ref(false)
const formValid = ref(false)

// Selected period
const selectedPeriod = ref('current_month')

// Advanced filters
const advancedFilters = ref<TransactionFilter>({
  dateFrom: '',
  dateTo: '',
  amountMin: null,
  amountMax: null,
  type: null,
  description: ''
})

// Form data
const transactionForm = ref<TransactionFormDto>({
  description: '',
  amount: 0,
  type: '',
  date: new Date().toISOString().split('T')[0] || '',
  accountId: accountId,
  categoryId: '',
  notes: '',
  status: 4 // Charged
})

// Period options
const periodOptions = [
  { title: 'Mese Corrente', value: 'current_month' },
  { title: 'Mese Precedente', value: 'last_month' },
  { title: 'Mese Successivo', value: 'next_month' },
  { title: 'Ultimi 3 Mesi', value: 'last_3_months' },
  { title: 'Anno Corrente', value: 'current_year' },
  { title: 'Tutto', value: 'all' }
]

// Transaction types
const transactionTypes = getTransactionTypeSelectOptions()

// Table headers
const headers = [
  { title: 'Data', key: 'date', sortable: true },
  { title: 'Descrizione', key: 'description', sortable: true },
  { title: 'Importo', key: 'amount', sortable: true },
  { title: 'Tipo', key: 'type', sortable: true },
  { title: 'Stato', key: 'status', sortable: true },
  { title: 'Categoria', key: 'category', sortable: false },
  { title: 'Ricorrente', key: 'isRecurring', sortable: true },
  { title: 'Azioni', key: 'actions', sortable: false }
]

// Snackbar
const snackbar = ref({
  show: false,
  message: '',
  color: 'success'
})

// Computed statistics
const statistics = computed(() => {
  const income = transactionList.value
    .filter(t => !t.isEstimated && (t.type === 'Income' || t.type === 1))
    .reduce((sum, t) => sum + t.amount, 0)
  
  const expenses = transactionList.value
    .filter(t => !t.isEstimated && (t.type === 'Expense' || t.type === 2))
    .reduce((sum, t) => sum + t.amount, 0)
  
  return {
    totalIncome: income,
    totalExpenses: expenses,
    netAmount: income - expenses,
    incomeCount: transactionList.value.filter(t => !t.isEstimated && (t.type === 'Income' || t.type === 1)).length,
    expenseCount: transactionList.value.filter(t => !t.isEstimated && (t.type === 'Expense' || t.type === 2)).length
  }
})

// Computed forecast statistics
const forecastStats = computed(() => {
  const expectedIncome = transactionList.value
    .filter(t => t.isEstimated && (t.type === 'Income' || t.type === 1))
    .reduce((sum, t) => sum + t.amount, 0)
  
  const expectedExpenses = transactionList.value
    .filter(t => t.isEstimated && (t.type === 'Expense' || t.type === 2))
    .reduce((sum, t) => sum + t.amount, 0)
  
  return {
    expectedIncome,
    expectedExpenses,
    expectedNet: expectedIncome - expectedExpenses
  }
})

// Computed forecast list
const forecast = computed(() => {
  return transactionList.value.filter(t => t.isEstimated)
})

// Methods
const loadAccount = async () => {
  try {
    loading.value = true
    const data = await getAccount(accountId)
    account.value = data
  } catch (error) {
    showSnackbar('Errore nel caricamento dell\'account', 'error')
  } finally {
    loading.value = false
  }
}

const loadTransactions = async () => {
  try {
    transactionsLoading.value = true
    const filters = buildFilters()
    
    // Use the new combined method to get both transactions and recurring
    const combinedData = await getCombinedTransactionList(filters)
    transactionList.value = combinedData
    
  } catch (error) {
    showSnackbar('Errore nel caricamento delle transazioni', 'error')
  } finally {
    transactionsLoading.value = false
  }
}

const loadCategoryOptions = async () => {
  try {
    const categories = await getCategorySelectOptions()
    categoryOptions.value = categories
  } catch (error) {
    showSnackbar('Errore nel caricamento delle categorie', 'error')
  }
}

const buildFilters = (): TransactionFilter => {
  const filters: TransactionFilter = {
    accountId: accountId
  }
  
  // Apply period filter
  const now = new Date()
  switch (selectedPeriod.value) {
    case 'current_month':
      filters.dateFrom = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().split('T')[0]
      filters.dateTo = new Date(now.getFullYear(), now.getMonth() + 1, 0).toISOString().split('T')[0]
      break
    case 'last_month':
      const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1)
      filters.dateFrom = lastMonth.toISOString().split('T')[0]
      filters.dateTo = new Date(now.getFullYear(), now.getMonth(), 0).toISOString().split('T')[0]
      break
    case 'next_month':
      filters.dateFrom = new Date(now.getFullYear(), now.getMonth() + 1, 1).toISOString().split('T')[0]
      filters.dateTo = new Date(now.getFullYear(), now.getMonth() + 2, 0).toISOString().split('T')[0]
      break
    case 'last_3_months':
      filters.dateFrom = new Date(now.getFullYear(), now.getMonth() - 2, 1).toISOString().split('T')[0]
      filters.dateTo = now.toISOString().split('T')[0]
      break
    case 'current_year':
      filters.dateFrom = new Date(now.getFullYear(), 0, 1).toISOString().split('T')[0]
      filters.dateTo = new Date(now.getFullYear(), 11, 31).toISOString().split('T')[0]
      break
  }
  
  // Apply advanced filters
  if (advancedFilters.value.dateFrom) {
    filters.dateFrom = advancedFilters.value.dateFrom
  }
  if (advancedFilters.value.dateTo) {
    filters.dateTo = advancedFilters.value.dateTo
  }
  if (advancedFilters.value.amountMin) {
    filters.amountMin = advancedFilters.value.amountMin
  }
  if (advancedFilters.value.amountMax) {
    filters.amountMax = advancedFilters.value.amountMax
  }
  if (advancedFilters.value.type) {
    filters.type = advancedFilters.value.type
  }
  if (advancedFilters.value.description) {
    filters.description = advancedFilters.value.description
  }
  
  return filters
}

const onPeriodChange = () => {
  loadTransactions()
}

const applyAdvancedFilters = () => {
  advancedFiltersDialog.value = false
  loadTransactions()
}

const clearAdvancedFilters = () => {
  advancedFilters.value = {
    dateFrom: '',
    dateTo: '',
    amountMin: null,
    amountMax: null,
    type: null,
    description: ''
  }
}

const openTransactionDialog = (transaction: TransactionListItem | null = null) => {
  editingTransaction.value = transaction
  if (transaction && !transaction.isRecurring) {
    transactionForm.value = { 
      description: transaction.description,
      amount: transaction.amount,
      type: String(transaction.type),
      date: (transaction.date?.split('T')[0] || new Date().toISOString().split('T')[0]) || '',
      accountId: accountId,
      categoryId: transaction.category?.id || '',
      notes: transaction.notes || '',
      status: typeof transaction.status === 'number' ? transaction.status : 4
    }
  } else {
    transactionForm.value = {
      description: '',
      amount: 0,
      type: '',
      date: new Date().toISOString().split('T')[0] || '',
      accountId: accountId,
      categoryId: '',
      notes: '',
      status: 4 // Charged
    }
  }
  transactionDialog.value = true
}

const closeTransactionDialog = () => {
  transactionDialog.value = false
  editingTransaction.value = null
}

const saveTransaction = async () => {
  if (!formValid.value) return

  try {
    submitting.value = true
    
    if (editingTransaction.value && !editingTransaction.value.isRecurring) {
      await updateTransaction(editingTransaction.value.id, transactionForm.value)
      showSnackbar('Transazione aggiornata con successo', 'success')
    } else {
      await createTransaction(transactionForm.value)
      showSnackbar('Transazione registrata con successo', 'success')
    }
    
    closeTransactionDialog()
    await loadTransactions()
  } catch (error) {
    showSnackbar('Errore nel salvataggio della transazione', 'error')
  } finally {
    submitting.value = false
  }
}

const confirmDeleteTransaction = (transaction: TransactionListItem) => {
  // Only allow deletion of regular transactions, not recurring templates
  if (transaction.isRecurring) {
    showSnackbar('Impossibile eliminare una ricorrenza da qui. Gestiscila dalla sezione Ricorrenze.', 'warning')
    return
  }
  
  transactionToDelete.value = transaction
  deleteDialog.value = true
}

const deleteTransactionHandler = async () => {
  if (!transactionToDelete.value || transactionToDelete.value.isRecurring) return

  try {
    deleting.value = true
    await deleteTransactionApi(transactionToDelete.value.id)
    showSnackbar('Transazione eliminata con successo', 'success')
    deleteDialog.value = false
    transactionToDelete.value = null
    await loadTransactions()
  } catch (error) {
    showSnackbar('Errore nell\'eliminazione della transazione', 'error')
  } finally {
    deleting.value = false
  }
}

const showSnackbar = (message: string, color: string = 'success') => {
  snackbar.value = {
    show: true,
    message,
    color
  }
}

// Month label helper
const getMonthLabel = (period: string) => {
  return getPeriodLabel(period)
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadAccount(),
    loadCategoryOptions(),
    loadTransactions()
  ])
})
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>