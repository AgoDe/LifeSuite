<template>
  <div>
    <!-- Header Section -->
    <v-row class="mb-4">
      <v-col>
        <h2 class="text-h4 mb-2">Gestione Categorie</h2>
        <p class="text-subtitle-1 text-medium-emphasis">
          Organizza le tue transazioni con categorie personalizzate
        </p>
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          prepend-icon="mdi-plus"
          @click="openCategoryDialog()"
        >
          Nuova Categoria
        </v-btn>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-skeleton-loader
      v-if="loading"
      type="table"
      class="mx-auto"
    />

    <!-- Categories Cards -->
    <v-row v-else>
      <v-col
        v-for="category in categories"
        :key="category.id"
        cols="12"
        md="6"
        lg="4"
      >
        <v-card class="h-100">
          <v-card-title class="d-flex align-center">
            <v-icon
              :color="category.color || 'primary'"
              class="mr-2"
            >
              {{ category.icon || 'mdi-tag' }}
            </v-icon>
            {{ category.name }}
          </v-card-title>
          
          <v-card-subtitle>
            {{ category.description }}
          </v-card-subtitle>

          <v-card-text>
            <v-chip
              :color="getTransactionTypeColor(category.transactionType)"
              size="small"
              variant="tonal"
              class="mb-2"
            >
              {{ getTransactionTypeLabel(category.transactionType) }}
            </v-chip>
            
            <div v-if="category.parentCategory" class="text-caption text-medium-emphasis">
              Sottocategoria di: {{ category.parentCategory.name }}
            </div>
          </v-card-text>

          <v-card-actions>
            <v-btn
              variant="text"
              color="secondary"
              @click="openCategoryDialog(category)"
            >
              Modifica
            </v-btn>
            <v-spacer />
            <v-btn
              variant="text"
              color="error"
              icon="mdi-delete"
              @click="confirmDelete(category)"
            />
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- Empty State -->
      <v-col v-if="!loading && categories.length === 0" cols="12">
        <v-card class="text-center pa-8">
          <v-icon size="64" color="grey-lighten-2" class="mb-4">
            mdi-tag-plus
          </v-icon>
          <h3 class="text-h6 mb-2">Nessuna categoria trovata</h3>
          <p class="text-body-2 text-medium-emphasis mb-4">
            Inizia creando la tua prima categoria per organizzare le transazioni
          </p>
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            @click="openCategoryDialog()"
          >
            Crea prima categoria
          </v-btn>
        </v-card>
      </v-col>
    </v-row>

    <!-- Category Dialog -->
    <v-dialog v-model="categoryDialog" max-width="500px">
      <v-card>
        <v-card-title>
          {{ editingCategory ? 'Modifica Categoria' : 'Nuova Categoria' }}
        </v-card-title>

        <v-card-text>
          <v-form ref="categoryFormRef" v-model="formValid">
            <v-text-field
              v-model="categoryForm.name"
              label="Nome Categoria"
              :rules="[v => !!v || 'Nome obbligatorio']"
              required
            />

            <v-textarea
              v-model="categoryForm.description"
              label="Descrizione"
              rows="2"
            />

            <v-select
              v-model="categoryForm.transactionType"
              :items="transactionTypes"
              label="Tipo Transazione"
              :rules="[v => !!v || 'Tipo obbligatorio']"
              required
            />

            <v-select
              v-model="categoryForm.icon"
              :items="iconOptions"
              label="Icona"
            >
              <template v-slot:selection="{ item }">
                <v-icon class="mr-2">{{ item.value }}</v-icon>
                {{ item.title }}
              </template>
              <template v-slot:item="{ props, item }">
                <v-list-item v-bind="props">
                  <template v-slot:prepend>
                    <v-icon>{{ item.value }}</v-icon>
                  </template>
                </v-list-item>
              </template>
            </v-select>

            <v-select
              v-model="categoryForm.color"
              :items="colorOptions"
              label="Colore"
            >
              <template v-slot:selection="{ item }">
                <v-icon :color="item.value" class="mr-2">mdi-circle</v-icon>
                {{ item.title }}
              </template>
              <template v-slot:item="{ props, item }">
                <v-list-item v-bind="props">
                  <template v-slot:prepend>
                    <v-icon :color="item.value">mdi-circle</v-icon>
                  </template>
                </v-list-item>
              </template>
            </v-select>
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn @click="closeCategoryDialog">Annulla</v-btn>
          <v-btn
            color="primary"
            :loading="submitting"
            @click="saveCategory"
          >
            {{ editingCategory ? 'Aggiorna' : 'Crea' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400px">
      <v-card>
        <v-card-title>Conferma Eliminazione</v-card-title>
        <v-card-text>
          Sei sicuro di voler eliminare la categoria "{{ categoryToDelete?.name }}"?
          Questa azione non può essere annullata.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="deleteDialog = false">Annulla</v-btn>
          <v-btn
            color="error"
            :loading="deleting"
            @click="deleteCategoryHandler"
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

const { getCategories, createCategory, updateCategory, deleteCategory: deleteCategoryApi } = useBudgetManager()

// Reactive state
const categories = ref<any[]>([])
const loading = ref(false)
const categoryDialog = ref(false)
const deleteDialog = ref(false)
const editingCategory = ref<any>(null)
const categoryToDelete = ref<any>(null)
const submitting = ref(false)
const deleting = ref(false)
const formValid = ref(false)

// Form data
const categoryForm = ref({
  name: '',
  description: '',
  transactionType: '',
  icon: 'mdi-tag',
  color: 'primary'
})

// Transaction types
const transactionTypes = [
  { title: 'Entrata', value: 'Income' },
  { title: 'Uscita', value: 'Expense' },
  { title: 'Trasferimento', value: 'Transfer' }
]

// Icon options
const iconOptions = [
  { title: 'Tag', value: 'mdi-tag' },
  { title: 'Cibo', value: 'mdi-food' },
  { title: 'Casa', value: 'mdi-home' },
  { title: 'Trasporto', value: 'mdi-car' },
  { title: 'Intrattenimento', value: 'mdi-movie' },
  { title: 'Salute', value: 'mdi-medical-bag' },
  { title: 'Istruzione', value: 'mdi-school' },
  { title: 'Shopping', value: 'mdi-shopping' },
  { title: 'Viaggio', value: 'mdi-airplane' },
  { title: 'Sport', value: 'mdi-run' },
  { title: 'Lavoro', value: 'mdi-briefcase' },
  { title: 'Regalo', value: 'mdi-gift' }
]

// Color options
const colorOptions = [
  { title: 'Primario', value: 'primary' },
  { title: 'Verde', value: 'green' },
  { title: 'Blu', value: 'blue' },
  { title: 'Arancione', value: 'orange' },
  { title: 'Rosso', value: 'red' },
  { title: 'Viola', value: 'purple' },
  { title: 'Teal', value: 'teal' },
  { title: 'Rosa', value: 'pink' }
]

// Snackbar
const snackbar = ref({
  show: false,
  message: '',
  color: 'success'
})

// Form ref
const categoryFormRef = ref(null)

// Methods
const loadCategories = async () => {
  try {
    loading.value = true
    const data = await getCategories()
    categories.value = data
  } catch (error) {
    showSnackbar('Errore nel caricamento delle categorie', 'error')
  } finally {
    loading.value = false
  }
}

const openCategoryDialog = (category: any = null) => {
  editingCategory.value = category
  if (category) {
    categoryForm.value = { ...category }
  } else {
    categoryForm.value = {
      name: '',
      description: '',
      transactionType: '',
      icon: 'mdi-tag',
      color: 'primary'
    }
  }
  categoryDialog.value = true
}

const closeCategoryDialog = () => {
  categoryDialog.value = false
  editingCategory.value = null
  categoryForm.value = {
    name: '',
    description: '',
    transactionType: '',
    icon: 'mdi-tag',
    color: 'primary'
  }
}

const saveCategory = async () => {
  if (!formValid.value) return

  try {
    submitting.value = true
    
    if (editingCategory.value) {
      await updateCategory(editingCategory.value.id, categoryForm.value)
      showSnackbar('Categoria aggiornata con successo', 'success')
    } else {
      await createCategory(categoryForm.value)
      showSnackbar('Categoria creata con successo', 'success')
    }
    
    closeCategoryDialog()
    await loadCategories()
  } catch (error) {
    showSnackbar('Errore nel salvataggio della categoria', 'error')
  } finally {
    submitting.value = false
  }
}

const confirmDelete = (category: any) => {
  categoryToDelete.value = category
  deleteDialog.value = true
}

const deleteCategoryHandler = async () => {
  if (!categoryToDelete.value) return

  try {
    deleting.value = true
    await deleteCategoryApi(categoryToDelete.value.id)
    showSnackbar('Categoria eliminata con successo', 'success')
    deleteDialog.value = false
    categoryToDelete.value = null
    await loadCategories()
  } catch (error) {
    showSnackbar('Errore nell\'eliminazione della categoria', 'error')
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

const getTransactionTypeLabel = (type: string) => {
  const labels = {
    'Income': 'Entrata',
    'Expense': 'Uscita',
    'Transfer': 'Trasferimento'
  }
  return labels[type] || type
}

const getTransactionTypeColor = (type: string) => {
  const colors = {
    'Income': 'green',
    'Expense': 'red',
    'Transfer': 'blue'
  }
  return colors[type] || 'primary'
}

// Lifecycle
onMounted(() => {
  loadCategories()
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