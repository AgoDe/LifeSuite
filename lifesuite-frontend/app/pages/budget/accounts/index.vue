<template>
  <div>
    <!-- Header Section -->
    <v-row class="mb-4">
      <v-col>
        <h2 class="text-h4 mb-2">Gestione Account</h2>
        <p class="text-subtitle-1 text-medium-emphasis">
          Gestisci i tuoi conti bancari e di risparmio
        </p>
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          prepend-icon="mdi-plus"
          @click="openAccountDialog()"
        >
          Nuovo Account
        </v-btn>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-skeleton-loader
      v-if="loading"
      type="table"
      class="mx-auto"
    />

    <!-- Accounts Cards -->
    <v-row v-else>
      <v-col
        v-for="account in accounts"
        :key="account.id"
        cols="12"
        md="6"
        lg="4"
      >
        <v-card class="h-100">
          <v-card-title class="d-flex align-center">
            <v-icon
              :color="getAccountTypeColor(account.type)"
              class="mr-2"
            >
              {{ getAccountTypeIcon(account.type) }}
            </v-icon>
            {{ account.name }}
          </v-card-title>
          
          <v-card-subtitle>
            {{ account.description }}
          </v-card-subtitle>

          <v-card-text>
            <div class="text-h6 mb-2">
              {{ formatCurrency(account.balance) }}
            </div>
            <v-chip
              :color="getAccountTypeColor(account.type)"
              size="small"
              variant="tonal"
            >
              {{ account.type }}
            </v-chip>
          </v-card-text>

          <v-card-actions>
            <v-btn
              variant="text"
              color="primary"
              :to="`/budget/accounts/${account.id}`"
            >
              Dettagli
            </v-btn>
            <v-btn
              variant="text"
              color="secondary"
              @click="openAccountDialog(account)"
            >
              Modifica
            </v-btn>
            <v-spacer />
            <v-btn
              variant="text"
              color="error"
              icon="mdi-delete"
              @click="confirmDelete(account)"
            />
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- Empty State -->
      <v-col v-if="!loading && accounts.length === 0" cols="12">
        <v-card class="text-center pa-8">
          <v-icon size="64" color="grey-lighten-2" class="mb-4">
            mdi-bank-plus
          </v-icon>
          <h3 class="text-h6 mb-2">Nessun account trovato</h3>
          <p class="text-body-2 text-medium-emphasis mb-4">
            Inizia creando il tuo primo account bancario
          </p>
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            @click="openAccountDialog()"
          >
            Crea primo account
          </v-btn>
        </v-card>
      </v-col>
    </v-row>

    <!-- Account Dialog -->
    <v-dialog v-model="accountDialog" max-width="500px">
      <v-card>
        <v-card-title>
          {{ editingAccount ? 'Modifica Account' : 'Nuovo Account' }}
        </v-card-title>

        <v-card-text>
          <v-form ref="accountFormRef" v-model="formValid">
            <v-text-field
              v-model="accountForm.name"
              label="Nome Account"
              :rules="[v => !!v || 'Nome obbligatorio']"
              required
            />

            <v-textarea
              v-model="accountForm.description"
              label="Descrizione"
              rows="2"
            />

            <v-select
              v-model="accountForm.type"
              :items="accountTypes"
              label="Tipo Account"
              :rules="[v => !!v || 'Tipo obbligatorio']"
              required
            />

            <v-text-field
              v-model="accountForm.balance"
              label="Saldo Iniziale"
              type="number"
              step="0.01"
              prefix="€"
              :rules="[v => v !== null && v !== '' || 'Saldo obbligatorio']"
              required
            />
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn @click="closeAccountDialog">Annulla</v-btn>
          <v-btn
            color="primary"
            :loading="submitting"
            @click="saveAccount"
          >
            {{ editingAccount ? 'Aggiorna' : 'Crea' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400px">
      <v-card>
        <v-card-title>Conferma Eliminazione</v-card-title>
        <v-card-text>
          Sei sicuro di voler eliminare l'account "{{ accountToDelete?.name }}"?
          Questa azione non può essere annullata.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="deleteDialog = false">Annulla</v-btn>
          <v-btn
            color="error"
            :loading="deleting"
            @click="deleteAccountHandler"
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
  layout: 'default'
  // middleware: 'auth' // Temporarily disabled for demo
})

const { getAccounts, createAccount, updateAccount, deleteAccount: deleteAccountApi } = useBudgetManager()

// Reactive state
const accounts = ref<any[]>([])
const loading = ref(false)
const accountDialog = ref(false)
const deleteDialog = ref(false)
const editingAccount = ref<any>(null)
const accountToDelete = ref<any>(null)
const submitting = ref(false)
const deleting = ref(false)
const formValid = ref(false)

// Form data
const accountForm = ref({
  name: '',
  description: '',
  type: '',
  balance: 0
})

// Account types
const accountTypes = [
  { title: 'Conto Corrente', value: 'Checking' },
  { title: 'Conto Risparmio', value: 'Savings' },
  { title: 'Carta di Credito', value: 'Credit' },
  { title: 'Investimenti', value: 'Investment' },
  { title: 'Contanti', value: 'Cash' }
]

// Snackbar
const snackbar = ref({
  show: false,
  message: '',
  color: 'success'
})

// Form ref
const accountFormRef = ref(null)

// Methods
const loadAccounts = async () => {
  try {
    loading.value = true
    const data = await getAccounts()
    accounts.value = data
  } catch (error) {
    showSnackbar('Errore nel caricamento degli account', 'error')
  } finally {
    loading.value = false
  }
}

const openAccountDialog = (account: any = null) => {
  editingAccount.value = account
  if (account) {
    accountForm.value = { ...account }
  } else {
    accountForm.value = {
      name: '',
      description: '',
      type: '',
      balance: 0
    }
  }
  accountDialog.value = true
}

const closeAccountDialog = () => {
  accountDialog.value = false
  editingAccount.value = null
  accountForm.value = {
    name: '',
    description: '',
    type: '',
    balance: 0
  }
}

const saveAccount = async () => {
  if (!formValid.value) return

  try {
    submitting.value = true
    
    if (editingAccount.value) {
      await updateAccount(editingAccount.value.id, accountForm.value)
      showSnackbar('Account aggiornato con successo', 'success')
    } else {
      await createAccount(accountForm.value)
      showSnackbar('Account creato con successo', 'success')
    }
    
    closeAccountDialog()
    await loadAccounts()
  } catch (error) {
    showSnackbar('Errore nel salvataggio dell\'account', 'error')
  } finally {
    submitting.value = false
  }
}

const confirmDelete = (account: any) => {
  accountToDelete.value = account
  deleteDialog.value = true
}

const deleteAccountHandler = async () => {
  if (!accountToDelete.value) return

  try {
    deleting.value = true
    await deleteAccountApi(accountToDelete.value.id)
    showSnackbar('Account eliminato con successo', 'success')
    deleteDialog.value = false
    accountToDelete.value = null
    await loadAccounts()
  } catch (error) {
    showSnackbar('Errore nell\'eliminazione dell\'account', 'error')
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

const getAccountTypeIcon = (type: string) => {
  const icons = {
    'Checking': 'mdi-bank',
    'Savings': 'mdi-piggy-bank',
    'Credit': 'mdi-credit-card',
    'Investment': 'mdi-trending-up',
    'Cash': 'mdi-cash'
  }
  return icons[type] || 'mdi-bank'
}

const getAccountTypeColor = (type: string) => {
  const colors = {
    'Checking': 'primary',
    'Savings': 'green',
    'Credit': 'orange',
    'Investment': 'blue',
    'Cash': 'teal'
  }
  return colors[type] || 'primary'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('it-IT', {
    style: 'currency',
    currency: 'EUR'
  }).format(amount)
}

// Lifecycle
onMounted(() => {
  loadAccounts()
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