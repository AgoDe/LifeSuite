<template>
  <div>
    <!-- Header Section -->
    <v-row class="mb-4">
      <v-col>
        <h2 class="text-h4 mb-2">Gestione Transazioni</h2>
        <p class="text-subtitle-1 text-medium-emphasis">
          Registra e gestisci tutte le tue transazioni finanziarie
        </p>
      </v-col>
      <v-col cols="auto" class="d-flex gap-2">
        <v-btn
          color="secondary"
          prepend-icon="mdi-filter"
          @click="filtersDialog = true"
        >
          Filtri
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

    <!-- Quick Filters -->
    <v-row class="mb-4">
      <v-col cols="12" md="3">
        <v-select
          v-model="quickFilters.account"
          :items="accountOptions"
          label="Account"
          prepend-icon="mdi-bank"
          clearable
          @update:model-value="applyQuickFilters"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="quickFilters.category"
          :items="categoryOptions"
          label="Categoria"
          prepend-icon="mdi-tag"
          clearable
          @update:model-value="applyQuickFilters"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="quickFilters.type"
          :items="transactionTypes"
          label="Tipo"
          prepend-icon="mdi-swap-horizontal"
          clearable
          @update:model-value="applyQuickFilters"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="quickFilters.period"
          :items="periodOptions"
          label="Periodo"
          prepend-icon="mdi-calendar"
          @update:model-value="applyQuickFilters"
        />
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-skeleton-loader
      v-if="loading"
      type="table"
      class="mx-auto"
    />

    <!-- Transactions Table -->
    <v-card v-else>
      <v-data-table
        :headers="headers"
        :items="transactions"
        :loading="loading"
        class="elevation-1"
        items-per-page="25"
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
          <v-icon
            :color="getTransactionTypeColor(item.type)"
            size="small"
          >
            {{ getTransactionTypeIcon(item.type) }}
          </v-icon>
          {{ getTransactionTypeLabel(item.type) }}
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
        </template>

        <template v-slot:item.account="{ item }">
          <div v-if="item.account" class="d-flex align-center">
            <v-icon size="small" class="mr-1">
              {{ getAccountTypeIcon(item.account.type) }}
            </v-icon>
            {{ item.account.name }}
          </div>
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
            @click="confirmDelete(item)"
          />
        </template>

        <template v-slot:no-data>
          <div class="text-center pa-8">
            <v-icon size="64" color="grey-lighten-2" class="mb-4">
              mdi-swap-horizontal
            </v-icon>
            <h3 class="text-h6 mb-2">Nessuna transazione trovata</h3>
            <p class="text-body-2 text-medium-emphasis mb-4">
              Inizia registrando la tua prima transazione
            </p>
            <v-btn
              color="primary"
              prepend-icon="mdi-plus"
              @click="openTransactionDialog()"
            >
              Registra transazione
            </v-btn>
          </div>
        </template>
      </v-data-table>
    </v-card>

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

            <v-row>
              <v-col cols="12" md="6">
                <v-select
                  v-model="transactionForm.accountId"
                  :items="accountOptions"
                  label="Account"
                  :rules="[v => !!v || 'Account obbligatorio']"
                  required
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-select
                  v-model="transactionForm.categoryId"
                  :items="categoryOptions"
                  label="Categoria"
                />
              </v-col>
            </v-row>

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

    <!-- Advanced Filters Dialog -->
    <v-dialog v-model="filtersDialog" max-width="500px">
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
          <v-text-field
            v-model="advancedFilters.description"
            label="Cerca nella descrizione"
            prepend-icon="mdi-magnify"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn @click="clearAdvancedFilters">Reset</v-btn>
          <v-spacer />
          <v-btn @click="filtersDialog = false">Annulla</v-btn>
          <v-btn color="primary" @click="applyAdvancedFilters">Applica</v-btn>
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

const { 
  getTransactions, 
  createTransaction, 
  updateTransaction, 
  deleteTransaction: deleteTransactionApi,
  getAccountSelectOptions,
  getCategorySelectOptions
} = useBudgetManager()

// Reactive state
const transactions = ref<any[]>([])
const accountOptions = ref<any[]>([])
const categoryOptions = ref<any[]>([])
const loading = ref(false)
const transactionDialog = ref(false)
const filtersDialog = ref(false)
const deleteDialog = ref(false)
const editingTransaction = ref<any>(null)
const transactionToDelete = ref<any>(null)
const submitting = ref(false)
const deleting = ref(false)
const formValid = ref(false)

// Quick filters
const quickFilters = ref({
  account: null,
  category: null,
  type: null,
  period: 'current_month'
})

// Advanced filters
const advancedFilters = ref({
  dateFrom: '',
  dateTo: '',
  amountMin: null,
  amountMax: null,
  description: ''
})

// Form data
const transactionForm = ref({
  description: '',
  amount: 0,
  type: '',
  date: new Date().toISOString().split('T')[0],
  accountId: '',
  categoryId: '',
  notes: ''
})

// Table headers
const headers = [
  { title: 'Data', key: 'date', sortable: true },
  { title: 'Descrizione', key: 'description', sortable: true },
  { title: 'Importo', key: 'amount', sortable: true },
  { title: 'Tipo', key: 'type', sortable: true },
  { title: 'Categoria', key: 'category', sortable: false },
  { title: 'Account', key: 'account', sortable: false },
  { title: 'Azioni', key: 'actions', sortable: false }
]

// Transaction types
const transactionTypes = [
  { title: 'Entrata', value: 'Income' },
  { title: 'Uscita', value: 'Expense' },
  { title: 'Trasferimento', value: 'Transfer' }
]

// Period options
const periodOptions = [
  { title: 'Mese Corrente', value: 'current_month' },
  { title: 'Mese Precedente', value: 'last_month' },
  { title: 'Ultimi 3 Mesi', value: 'last_3_months' },
  { title: 'Anno Corrente', value: 'current_year' },
  { title: 'Tutto', value: 'all' }
]

// Snackbar
const snackbar = ref({
  show: false,
  message: '',
  color: 'success'
})

// Form ref
const transactionFormRef = ref(null)

// Methods
const loadTransactions = async () => {
  try {
    loading.value = true
    const filters = buildFilters()
    const data = await getTransactions(filters)
    transactions.value = data
  } catch (error) {
    showSnackbar('Errore nel caricamento delle transazioni', 'error')
  } finally {
    loading.value = false
  }
}

const loadSelectOptions = async () => {
  try {
    const [accounts, categories] = await Promise.all([
      getAccountSelectOptions(),
      getCategorySelectOptions()
    ])
    accountOptions.value = accounts
    categoryOptions.value = categories
  } catch (error) {
    showSnackbar('Errore nel caricamento delle opzioni', 'error')
  }
}

const buildFilters = () => {
  const filters: any = {}
  
  if (quickFilters.value.account) {
    filters.accountId = quickFilters.value.account
  }
  
  if (quickFilters.value.category) {
    filters.categoryId = quickFilters.value.category
  }
  
  if (quickFilters.value.type) {
    filters.type = quickFilters.value.type
  }
  
  // Apply period filter
  const now = new Date()
  switch (quickFilters.value.period) {
    case 'current_month':
      filters.dateFrom = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().split('T')[0]
      filters.dateTo = new Date(now.getFullYear(), now.getMonth() + 1, 0).toISOString().split('T')[0]
      break
    case 'last_month':
      const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1)
      filters.dateFrom = lastMonth.toISOString().split('T')[0]
      filters.dateTo = new Date(now.getFullYear(), now.getMonth(), 0).toISOString().split('T')[0]
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
  if (advancedFilters.value.description) {
    filters.description = advancedFilters.value.description
  }
  
  return filters
}

const applyQuickFilters = () => {
  loadTransactions()
}

const applyAdvancedFilters = () => {
  filtersDialog.value = false
  loadTransactions()
}

const clearAdvancedFilters = () => {
  advancedFilters.value = {
    dateFrom: '',
    dateTo: '',
    amountMin: null,
    amountMax: null,
    description: ''
  }
}

const openTransactionDialog = (transaction: any = null) => {
  editingTransaction.value = transaction
  if (transaction) {
    transactionForm.value = { 
      ...transaction,
      date: transaction.date?.split('T')[0] || new Date().toISOString().split('T')[0]
    }
  } else {
    transactionForm.value = {
      description: '',
      amount: 0,
      type: '',
      date: new Date().toISOString().split('T')[0],
      accountId: '',
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

const confirmDelete = (transaction: any) => {
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

// Lifecycle
onMounted(async () => {
  await loadSelectOptions()
  await loadTransactions()
})
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>