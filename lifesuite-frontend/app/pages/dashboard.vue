<template>
  <div>
    <!-- Header della dashboard -->
    <v-row class="mb-6">
      <v-col cols="12">
        <h1 class="text-h4 font-weight-bold mb-2">
          Benvenuto, {{ user?.firstName || 'Utente' }}! 👋
        </h1>
        <p class="text-body-1 text-medium-emphasis">
          Ecco una panoramica della tua giornata e dei tuoi obiettivi.
        </p>
      </v-col>
    </v-row>

    <!-- Cards statistiche -->
    <v-row class="mb-6">
      <v-col
        v-for="stat in stats"
        :key="stat.title"
        cols="12"
        sm="6"
        lg="3"
      >
        <v-card
          class="pa-4"
          elevation="2"
          :color="stat.color"
          variant="flat"
        >
          <v-row no-gutters align="center">
            <v-col>
              <div class="text-h6 font-weight-medium text-white">
                {{ stat.title }}
              </div>
              <div class="text-h4 font-weight-bold text-white">
                {{ stat.value }}
              </div>
              <div class="text-body-2 text-white opacity-80">
                {{ stat.subtitle }}
              </div>
            </v-col>
            <v-col cols="auto">
              <v-icon size="48" color="white" class="opacity-80">
                {{ stat.icon }}
              </v-icon>
            </v-col>
          </v-row>
        </v-card>
      </v-col>
    </v-row>

    <!-- Contenuto principale -->
    <v-row>
      <!-- Tasks oggi -->
      <v-col cols="12" md="6">
        <v-card elevation="2">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-check-circle</v-icon>
            Task di Oggi
            <v-spacer />
            <v-chip size="small" color="primary">
              {{ todayTasks.filter(t => !t.completed).length }}
            </v-chip>
          </v-card-title>
          
          <v-card-text>
            <v-list>
              <v-list-item
                v-for="task in todayTasks"
                :key="task.id"
                class="px-0"
              >
                <template v-slot:prepend>
                  <v-checkbox
                    v-model="task.completed"
                    color="success"
                    @change="toggleTask(task)"
                  />
                </template>
                
                <v-list-item-title
                  :class="{ 'text-decoration-line-through text-medium-emphasis': task.completed }"
                >
                  {{ task.title }}
                </v-list-item-title>
                
                <template v-slot:append>
                  <v-chip
                    size="small"
                    :color="task.priority === 'high' ? 'error' : task.priority === 'medium' ? 'warning' : 'success'"
                    variant="tonal"
                  >
                    {{ task.priority }}
                  </v-chip>
                </template>
              </v-list-item>
            </v-list>
            
            <v-btn
              color="primary"
              variant="text"
              prepend-icon="mdi-plus"
              block
              class="mt-2"
              @click="addTask"
            >
              Aggiungi Task
            </v-btn>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Budget del mese -->
      <v-col cols="12" md="6">
        <v-card elevation="2">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-wallet</v-icon>
            Budget Mensile
            <v-spacer />
            <v-chip 
              size="small" 
              :color="budgetPercentage > 80 ? 'error' : budgetPercentage > 60 ? 'warning' : 'success'"
            >
              {{ budgetPercentage }}%
            </v-chip>
          </v-card-title>
          
          <v-card-text>
            <div class="mb-4">
              <div class="d-flex justify-space-between mb-2">
                <span class="font-weight-medium">€{{ budget.spent.toLocaleString() }}</span>
                <span class="text-medium-emphasis">€{{ budget.total.toLocaleString() }}</span>
              </div>
              
              <v-progress-linear
                :model-value="budgetPercentage"
                :color="budgetPercentage > 80 ? 'error' : budgetPercentage > 60 ? 'warning' : 'success'"
                height="8"
                rounded
              />
            </div>
            
            <v-list density="compact">
              <v-list-item
                v-for="category in budget.categories"
                :key="category.name"
                class="px-0"
              >
                <v-list-item-title>{{ category.name }}</v-list-item-title>
                <template v-slot:append>
                  <span class="font-weight-medium">
                    €{{ category.spent.toLocaleString() }}
                  </span>
                </template>
              </v-list-item>
            </v-list>
            
            <v-btn
              color="primary"
              variant="text"
              prepend-icon="mdi-plus"
              block
              class="mt-2"
              @click="addExpense"
            >
              Aggiungi Spesa
            </v-btn>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Eventi prossimi -->
    <v-row class="mt-6">
      <v-col cols="12">
        <v-card elevation="2">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-calendar</v-icon>
            Prossimi Eventi
            <v-spacer />
            <v-btn
              color="primary"
              variant="text"
              prepend-icon="mdi-plus"
              @click="addEvent"
            >
              Nuovo Evento
            </v-btn>
          </v-card-title>
          
          <v-card-text>
            <v-row>
              <v-col
                v-for="event in upcomingEvents"
                :key="event.id"
                cols="12"
                sm="6"
                md="4"
              >
                <v-card variant="outlined" class="pa-3">
                  <div class="d-flex align-center mb-2">
                    <v-icon :color="event.color" class="mr-2">
                      {{ event.icon }}
                    </v-icon>
                    <div class="font-weight-medium">{{ event.title }}</div>
                  </div>
                  <div class="text-body-2 text-medium-emphasis mb-1">
                    {{ event.date }}
                  </div>
                  <div class="text-body-2">
                    {{ event.time }}
                  </div>
                </v-card>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
// Imposta il layout per questa pagina
definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

const { user } = useAuth()

// Dati di esempio per la dashboard
const stats = ref([
  {
    title: 'Task Completati',
    value: '12',
    subtitle: 'Questa settimana',
    icon: 'mdi-check-circle',
    color: 'success'
  },
  {
    title: 'Budget Speso',
    value: '€1,250',
    subtitle: 'Questo mese',
    icon: 'mdi-wallet',
    color: 'warning'
  },
  {
    title: 'Eventi',
    value: '5',
    subtitle: 'Questa settimana',
    icon: 'mdi-calendar',
    color: 'info'
  },
  {
    title: 'Obiettivi',
    value: '3/5',
    subtitle: 'Completati',
    icon: 'mdi-target',
    color: 'primary'
  }
])

const todayTasks = ref([
  {
    id: 1,
    title: 'Completare il report mensile',
    completed: false,
    priority: 'high'
  },
  {
    id: 2,
    title: 'Chiamare il dottore',
    completed: true,
    priority: 'medium'
  },
  {
    id: 3,
    title: 'Fare la spesa',
    completed: false,
    priority: 'low'
  },
  {
    id: 4,
    title: 'Palestra',
    completed: false,
    priority: 'medium'
  }
])

const budget = ref({
  total: 2000,
  spent: 1250,
  categories: [
    { name: 'Alimentari', spent: 450 },
    { name: 'Trasporti', spent: 200 },
    { name: 'Intrattenimento', spent: 300 },
    { name: 'Bollette', spent: 300 }
  ]
})

const upcomingEvents = ref([
  {
    id: 1,
    title: 'Riunione Team',
    date: 'Oggi',
    time: '14:30',
    icon: 'mdi-account-group',
    color: 'primary'
  },
  {
    id: 2,
    title: 'Dentista',
    date: 'Domani',
    time: '10:00',
    icon: 'mdi-medical-bag',
    color: 'info'
  },
  {
    id: 3,
    title: 'Cena con amici',
    date: 'Venerdì',
    time: '19:30',
    icon: 'mdi-silverware',
    color: 'success'
  }
])

// Computed
const budgetPercentage = computed(() => {
  return Math.round((budget.value.spent / budget.value.total) * 100)
})

// Methods
const toggleTask = (task: any) => {
  console.log('Task toggled:', task)
  // Qui implementerai la logica per salvare il cambiamento
}

const addTask = () => {
  console.log('Aggiungi nuovo task')
  // Qui implementerai la logica per aggiungere un task
}

const addExpense = () => {
  console.log('Aggiungi nuova spesa')
  // Qui implementerai la logica per aggiungere una spesa
}

const addEvent = () => {
  console.log('Aggiungi nuovo evento')
  // Qui implementerai la logica per aggiungere un evento
}
</script>