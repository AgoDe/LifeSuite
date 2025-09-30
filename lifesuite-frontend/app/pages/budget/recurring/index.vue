<template>
  <div>
    <!-- Header Section -->
    <v-row class="mb-4">
      <v-col>
        <h2 class="text-h4 mb-2">Transazioni Ricorrenti</h2>
        <p class="text-subtitle-1 text-medium-emphasis">
          Automatizza le tue transazioni ricorrenti mensili
        </p>
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          prepend-icon="mdi-plus"
          @click="openRecurringDialog()"
        >
          Nuova Ricorrenza
        </v-btn>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-skeleton-loader
      v-if="loading"
      type="table"
      class="mx-auto"
    />

    <!-- Recurring Cards -->
    <v-row v-else>
      <v-col
        v-for="recurring in recurringTransactions"
        :key="recurring.id"
        cols="12"
        md="6"
        lg="4"
      >
        <v-card class="h-100">
          <v-card-title class="d-flex align-center">
            <v-icon
              :color="getTransactionTypeColor(recurring.transactionType)"
              class="mr-2"
            >
              {{ getTransactionTypeIcon(recurring.transactionType) }}
            </v-icon>
            {{ recurring.description }}
          </v-card-title>
          
          <v-card-subtitle>
            {{ recurring.notes }}
          </v-card-subtitle>

          <v-card-text>
            <div class="text-h6 mb-2">
              {{ formatCurrency(recurring.amount) }}
            </div>
            
            <v-chip
              :color="getTransactionTypeColor(recurring.transactionType)"
              size="small"
              variant="tonal"
              class="mb-2"
            >
              {{ getTransactionTypeLabel(recurring.transactionType) }}
            </v-chip>

            <div class="text-body-2 text-medium-emphasis mb-2">
              <v-icon size="16" class="mr-1">mdi-calendar-repeat</v-icon>
              {{ getFrequencyLabel(recurring.frequency) }}
            </div>

            <div v-if="recurring.nextDate" class="text-body-2 text-medium-emphasis mb-2">
              <v-icon size="16" class="mr-1">mdi-calendar-clock</v-icon>
              Prossima: {{ formatDate(recurring.nextDate) }}
            </div>

            <div v-if="recurring.account" class="text-body-2 text-medium-emphasis mb-2">
              <v-icon size="16" class="mr-1">{{ getAccountTypeIcon(recurring.account.type) }}</v-icon>
              {{ recurring.account.name }}
            </div>

            <div v-if="recurring.category" class="text-body-2 text-medium-emphasis">
              <v-icon 
                :color="recurring.category.color || 'primary'" 
                size="16" 
                class="mr-1"
              >
                {{ recurring.category.icon || 'mdi-tag' }}
              </v-icon>
              {{ recurring.category.name }}
            </div>
          </v-card-text>

          <v-card-actions>
            <v-chip
              :color="recurring.isActive ? 'green' : 'grey'"
              size="small"
              variant="tonal"
            >
              {{ recurring.isActive ? 'Attiva' : 'Inattiva' }}
            </v-chip>
            
            <v-spacer />
            
            <v-btn
              variant="text"
              color="secondary"
              icon="mdi-pencil"
              @click="openRecurringDialog(recurring)"
            />
            <v-btn
              variant="text"
              :color="recurring.isActive ? 'orange' : 'green'"
              :icon="recurring.isActive ? 'mdi-pause' : 'mdi-play'"
              @click="toggleRecurringStatus(recurring)"
            />
            <v-btn
              variant="text"
              color="error"
              icon="mdi-delete"
              @click="confirmDelete(recurring)"
            />
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- Empty State -->
      <v-col v-if="!loading && recurringTransactions.length === 0" cols="12">
        <v-card class="text-center pa-8">
          <v-icon size="64" color="grey-lighten-2" class="mb-4">
            mdi-repeat-variant
          </v-icon>
          <h3 class="text-h6 mb-2">Nessuna ricorrenza trovata</h3>
          <p class="text-body-2 text-medium-emphasis mb-4">
            Inizia creando la tua prima transazione ricorrente
          </p>
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            @click="openRecurringDialog()"
          >
            Crea prima ricorrenza
          </v-btn>
        </v-card>
      </v-col>
    </v-row>

    <!-- Recurring Dialog -->
    <v-dialog v-model="recurringDialog" max-width="600px">
      <v-card>
        <v-card-title>
          {{ editingRecurring ? 'Modifica Ricorrenza' : 'Nuova Ricorrenza' }}
        </v-card-title>

        <v-card-text>
          <v-form ref="recurringFormRef" v-model="formValid">
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="recurringForm.description"
                  label="Descrizione"
                  :rules="[v => !!v || 'Descrizione obbligatoria']"
                  required
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="recurringForm.amount"
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
                  v-model="recurringForm.transactionType"
                  :items="transactionTypes"
                  label="Tipo Transazione"
                  :rules="[v => !!v || 'Tipo obbligatorio']"
                  required
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-select
                  v-model="recurringForm.frequency"
                  :items="frequencyOptions"
                  label="Frequenza"
                  :rules="[v => !!v || 'Frequenza obbligatoria']"
                  required
                />
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <v-select
                  v-model="recurringForm.accountId"
                  :items="accountOptions"
                  label="Account"
                  :rules="[v => !!v || 'Account obbligatorio']"
                  required
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-select
                  v-model="recurringForm.categoryId"
                  :items="categoryOptions"
                  label="Categoria"
                />
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="recurringForm.startDate"
                  label="Data Inizio"
                  type="date"
                  :rules="[v => !!v || 'Data inizio obbligatoria']"
                  required
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="recurringForm.endDate"
                  label="Data Fine (opzionale)"
                  type="date"
                />
              </v-col>
            </v-row>

            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="recurringForm.dayOfMonth"
                  label="Giorno del Mese"
                  type="number"
                  min="1"
                  max="31"
                  hint="Giorno del mese per l'esecuzione (1-31)"
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-switch
                  v-model="recurringForm.isActive"
                  label="Attiva"
                  color="primary"
                />
              </v-col>
            </v-row>

            <v-textarea
              v-model="recurringForm.notes"
              label="Note"
              rows="2"
            />
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn @click="closeRecurringDialog">Annulla</v-btn>
          <v-btn
            color="primary"
            :loading="submitting"
            @click="saveRecurring"
          >
            {{ editingRecurring ? 'Aggiorna' : 'Crea' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400px">
      <v-card>
        <v-card-title>Conferma Eliminazione</v-card-title>
        <v-card-text>
          Sei sicuro di voler eliminare la ricorrenza "{{ recurringToDelete?.description }}"?
          Questa azione non può essere annullata.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="deleteDialog = false">Annulla</v-btn>
          <v-btn
            color="error"
            :loading="deleting"
            @click="deleteRecurringHandler"
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
  getRecurring, 
  createRecurring, 
  updateRecurring, 
  deleteRecurring: deleteRecurringApi,
  getAccountSelectOptions,
  getCategorySelectOptions
} = useBudgetManager()

// Reactive state
const recurringTransactions = ref<any[]>([])
const accountOptions = ref<any[]>([])
const categoryOptions = ref<any[]>([])
const loading = ref(false)
const recurringDialog = ref(false)
const deleteDialog = ref(false)
const editingRecurring = ref<any>(null)
const recurringToDelete = ref<any>(null)
const submitting = ref(false)
const deleting = ref(false)
const formValid = ref(false)

// Form data
const recurringForm = ref({
  description: '',
  amount: 0,
  transactionType: '',
  frequency: '',
  accountId: '',
  categoryId: '',
  startDate: new Date().toISOString().split('T')[0],
  endDate: '',
  dayOfMonth: 1,
  isActive: true,
  notes: ''
})

// Transaction types
const transactionTypes = [
  { title: 'Entrata', value: 'Income' },
  { title: 'Uscita', value: 'Expense' },
  { title: 'Trasferimento', value: 'Transfer' }
]

// Frequency options
const frequencyOptions = [
  { title: 'Mensile', value: 'Monthly' },
  { title: 'Settimanale', value: 'Weekly' },
  { title: 'Annuale', value: 'Yearly' }
]

// Snackbar
const snackbar = ref({
  show: false,
  message: '',
  color: 'success'
})

// Form ref
const recurringFormRef = ref(null)

// Methods
const loadRecurring = async () => {
  try {
    loading.value = true
    const data = await getRecurring()
    recurringTransactions.value = data
  } catch (error) {
    showSnackbar('Errore nel caricamento delle ricorrenze', 'error')
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

const openRecurringDialog = (recurring = null) => {
  editingRecurring.value = recurring
  if (recurring) {
    recurringForm.value = { 
      ...recurring,
      startDate: recurring.startDate?.split('T')[0] || new Date().toISOString().split('T')[0],
      endDate: recurring.endDate?.split('T')[0] || ''
    }
  } else {
    recurringForm.value = {
      description: '',
      amount: 0,
      transactionType: '',
      frequency: '',
      accountId: '',
      categoryId: '',
      startDate: new Date().toISOString().split('T')[0],
      endDate: '',
      dayOfMonth: 1,
      isActive: true,
      notes: ''
    }
  }
  recurringDialog.value = true
}

const closeRecurringDialog = () => {
  recurringDialog.value = false
  editingRecurring.value = null
}

const saveRecurring = async () => {
  if (!formValid.value) return

  try {
    submitting.value = true
    
    if (editingRecurring.value) {
      await updateRecurring(editingRecurring.value.id, recurringForm.value)
      showSnackbar('Ricorrenza aggiornata con successo', 'success')
    } else {
      await createRecurring(recurringForm.value)
      showSnackbar('Ricorrenza creata con successo', 'success')
    }
    
    closeRecurringDialog()
    await loadRecurring()
  } catch (error) {
    showSnackbar('Errore nel salvataggio della ricorrenza', 'error')
  } finally {
    submitting.value = false
  }
}

const toggleRecurringStatus = async (recurring) => {
  try {
    const updatedRecurring = {
      ...recurring,
      isActive: !recurring.isActive
    }
    
    await updateRecurring(recurring.id, updatedRecurring)
    showSnackbar(
      `Ricorrenza ${updatedRecurring.isActive ? 'attivata' : 'disattivata'} con successo`,
      'success'
    )
    await loadRecurring()
  } catch (error) {
    showSnackbar('Errore nell\'aggiornamento dello stato', 'error')
  }
}

const confirmDelete = (recurring) => {
  recurringToDelete.value = recurring
  deleteDialog.value = true
}

const deleteRecurringHandler = async () => {
  if (!recurringToDelete.value) return

  try {
    deleting.value = true
    await deleteRecurringApi(recurringToDelete.value.id)
    showSnackbar('Ricorrenza eliminata con successo', 'success')
    deleteDialog.value = false
    recurringToDelete.value = null
    await loadRecurring()
  } catch (error) {
    showSnackbar('Errore nell\'eliminazione della ricorrenza', 'error')
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

const getFrequencyLabel = (frequency: string) => {
  const labels : Record<string, string> = {
    'Monthly': 'Mensile',
    'Weekly': 'Settimanale',
    'Yearly': 'Annuale'
  }
  return labels[frequency] || frequency
}

const getTransactionTypeLabel = (type: string) => {
  const labels : Record<string, string> = {
    'Income': 'Entrata',
    'Expense': 'Uscita',
    'Transfer': 'Trasferimento'
  }
  return labels[type] || type
}

const getTransactionTypeColor = (type: string) => {
  const colors : Record<string, string> = {
    'Income': 'green',
    'Expense': 'red',
    'Transfer': 'blue'
  }
  return colors[type] || 'primary'
}

const getTransactionTypeIcon = (type: string) => {
  const icons : Record<string, string> = {
    'Income': 'mdi-arrow-down',
    'Expense': 'mdi-arrow-up',
    'Transfer': 'mdi-swap-horizontal'
  }
  return icons[type] || 'mdi-swap-horizontal'
}

const getAccountTypeIcon = (type: string) => {
  const icons : Record<string, string> = {
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
  await loadRecurring()
})
</script>

<style scoped>
.v-card {
  transition: transform 0.2s ease-in-out;
}

.v-card:hover {
  transform: translateY(-2px);
}
</style>