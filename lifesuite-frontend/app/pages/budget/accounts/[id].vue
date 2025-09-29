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
              :color="getAccountTypeColor(account?.type)"
              size="32"
              class="mr-3"
            >
              {{ getAccountTypeIcon(account?.type) }}
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
            :color="getAccountTypeColor(account?.type)"
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
          Transazioni
          <v-spacer />
          <span class="text-caption">{{ transactions.length }} transazioni</span>
        </v-card-title>
        
        <v-data-table
          :headers="headers"
          :items="transactions"
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

          <template v-slot:item.actions="{ item }">
            <v-btn
              variant="text"
              color="secondary"
              icon="mdi-pencil"
              size="small"
              @click="openTransactionDialog(item)"
            />
            <v-btn
              variant="text"
              color="error"
              icon="mdi-delete"
              size="small"
              @click="confirmDeleteTransaction(item)"
            />
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
definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

const route = useRoute()
const accountId = route.params.id as string

const { 
  getAccount,
  getTransactions,
  getTransactionsWithRecurrings,
  createTransaction,
  updateTransaction,
  deleteTransaction: deleteTransactionApi,
  getCategorySelectOptions
} = useBudgetManager()

// Reactive state
const account = ref<any>(null)
const transactions = ref<any[]>([])
const forecast = ref<any[]>([])
const categoryOptions = ref<any[]>([])
const loading = ref(false)
const transactionsLoading = ref(false)
const transactionDialog = ref(false)
const advancedFiltersDialog = ref(false)
const deleteDialog = ref(false)
const editingTransaction = ref<any>(null)
const transactionToDelete = ref<any>(null)
const submitting = ref(false)
const deleting = ref(false)
const formValid = ref(false)

// Selected period
const selectedPeriod = ref('current_month')

// Advanced filters
const advancedFilters = ref({
  dateFrom: '',
  dateTo: '',
  amountMin: null,
  amountMax: null,
  type: null,
  description: ''
})

// Form data
const transactionForm = ref({
  description: '',
  amount: 0,
  type: '',
  date: new Date().toISOString().split('T')[0],
  accountId: accountId,
  categoryId: '',
  notes: ''
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
const transactionTypes = [
  { title: 'Entrata', value: 'Income' },
  { title: 'Uscita', value: 'Expense' },
  { title: 'Trasferimento', value: 'Transfer' }
]

// Table headers
const headers = [
  { title: 'Data', key: 'date', sortable: true },
  { title: 'Descrizione', key: 'description', sortable: true },
  { title: 'Importo', key: 'amount', sortable: true },
  { title: 'Tipo', key: 'type', sortable: true },
  { title: 'Categoria', key: 'category', sortable: false },
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
  const income = transactions.value
    .filter(t => t.type === 'Income')
    .reduce((sum, t) => sum + t.amount, 0)
  
  const expenses = transactions.value
    .filter(t => t.type === 'Expense')
    .reduce((sum, t) => sum + t.amount, 0)
  
  return {
    totalIncome: income,
    totalExpenses: expenses,
    netAmount: income - expenses,
    incomeCount: transactions.value.filter(t => t.type === 'Income').length,
    expenseCount: transactions.value.filter(t => t.type === 'Expense').length
  }
})

// Computed forecast statistics
const forecastStats = computed(() => {
  const expectedIncome = forecast.value
    .filter(t => t.type === 'Income')
    .reduce((sum, t) => sum + t.amount, 0)
  
  const expectedExpenses = forecast.value
    .filter(t => t.type === 'Expense')
    .reduce((sum, t) => sum + t.amount, 0)
  
  return {
    expectedIncome,
    expectedExpenses,
    expectedNet: expectedIncome - expectedExpenses
  }
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
    
    // Load both regular transactions and forecast if applicable
    const [transactionsData, forecastData] = await Promise.all([
      getTransactions(filters),
      selectedPeriod.value.includes('next') || selectedPeriod.value === 'current_month'
        ? getTransactionsWithRecurrings(filters)
        : Promise.resolve([])
    ])
    
    transactions.value = transactionsData
    forecast.value = forecastData.filter ? forecastData.filter(t => t.isEstimated) : []
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

const buildFilters = () => {
  const filters: any = {
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

const openTransactionDialog = (transaction: any = null) => {
  editingTransaction.value = transaction
  if (transaction) {
    transactionForm.value = { 
      ...transaction,
      date: transaction.date?.split('T')[0] || new Date().toISOString().split('T')[0],
      accountId: accountId
    }
  } else {
    transactionForm.value = {
      description: '',
      amount: 0,
      type: '',
      date: new Date().toISOString().split('T')[0],
      accountId: accountId,
      categoryId: '',
      notes: ''
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
    
    if (editingTransaction.value) {
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

const confirmDeleteTransaction = (transaction: any) => {
  transactionToDelete.value = transaction
  deleteDialog.value = true
}

const deleteTransactionHandler = async () => {
  if (!transactionToDelete.value) return

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

// Helper functions
const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('it-IT')
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('it-IT', {
    style: 'currency',
    currency: 'EUR'
  }).format(amount)
}

const getAmountColor = (type: string, amount: number) => {
  if (type === 'Income') return 'green'
  if (type === 'Expense') return 'red'
  return 'blue'
}

const getTransactionTypeLabel = (type: string) => {
  const labels: Record<string, string> = {
    'Income': 'Entrata',
    'Expense': 'Uscita',
    'Transfer': 'Trasferimento'
  }
  return labels[type] || type
}

const getTransactionTypeColor = (type: string) => {
  const colors: Record<string, string> = {
    'Income': 'green',
    'Expense': 'red',
    'Transfer': 'blue'
  }
  return colors[type] || 'primary'
}

const getTransactionTypeIcon = (type: string) => {
  const icons: Record<string, string> = {
    'Income': 'mdi-arrow-down',
    'Expense': 'mdi-arrow-up',
    'Transfer': 'mdi-swap-horizontal'
  }
  return icons[type] || 'mdi-swap-horizontal'
}

const getAccountTypeIcon = (type: string) => {
  const icons: Record<string, string> = {
    'Checking': 'mdi-bank',
    'Savings': 'mdi-piggy-bank',
    'Credit': 'mdi-credit-card',
    'Investment': 'mdi-trending-up',
    'Cash': 'mdi-cash'
  }
  return icons[type] || 'mdi-bank'
}

const getAccountTypeColor = (type: string) => {
  const colors: Record<string, string> = {
    'Checking': 'primary',
    'Savings': 'green',
    'Credit': 'orange',
    'Investment': 'blue',
    'Cash': 'teal'
  }
  return colors[type] || 'primary'
}

const getMonthLabel = (period: string) => {
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