<template>
  <v-app>
    <v-app-bar elevation="1">
      <v-app-bar-title>LifeSuite Dashboard</v-app-bar-title>
      <v-spacer />
      <v-btn @click="handleLogout" variant="outlined">
        <v-icon start>mdi-logout</v-icon>
        Esci
      </v-btn>
    </v-app-bar>

    <v-main>
      <v-container>
        <v-row>
          <v-col cols="12">
            <h1 class="text-h3 mb-4">Benvenuto nella Dashboard!</h1>
            <p class="text-h6 text-medium-emphasis mb-6">
              Accesso completato con successo. Qui potrai gestire la tua vita quotidiana.
            </p>

            <v-row>
              <v-col cols="12" md="4">
                <v-card>
                  <v-card-title>
                    <v-icon start>mdi-calendar</v-icon>
                    Calendario
                  </v-card-title>
                  <v-card-text>
                    Gestisci i tuoi appuntamenti e eventi
                  </v-card-text>
                  <v-card-actions>
                    <v-btn color="primary">Apri Calendario</v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>

              <v-col cols="12" md="4">
                <v-card>
                  <v-card-title>
                    <v-icon start>mdi-check-circle</v-icon>
                    Todo List
                  </v-card-title>
                  <v-card-text>
                    Organizza le tue attività quotidiane
                  </v-card-text>
                  <v-card-actions>
                    <v-btn color="primary">Gestisci Todo</v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>

              <v-col cols="12" md="4">
                <v-card>
                  <v-card-title>
                    <v-icon start>mdi-account</v-icon>
                    Profilo
                  </v-card-title>
                  <v-card-text>
                    Gestisci le tue informazioni personali
                  </v-card-text>
                  <v-card-actions>
                    <v-btn color="primary">Vai al Profilo</v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
// Proteggi questa pagina - solo utenti autenticati
definePageMeta({
  middleware: 'auth'
})

const { logout, user } = useAuth()

const handleLogout = async () => {
  try {
    await logout()
  } catch (error) {
    console.error('Errore durante il logout:', error)
  }
}

// Verifica autenticazione all'avvio
onMounted(async () => {
  const { checkAuth } = useAuth()
  const isAuthenticated = await checkAuth()
  
  if (!isAuthenticated) {
    await navigateTo('/auth/login')
  }
})
</script>
