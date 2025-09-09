<template>
  <div>
    <div class="text-center mb-8">
      <h1 class="text-h4 font-weight-bold mb-2">Accedi</h1>
      <p class="text-body-1 text-medium-emphasis">Accedi al tuo account LifeSuite</p>
    </div>

    <v-form @submit.prevent="handleLogin" ref="form">
      <v-text-field
        v-model="form.email"
        label="Email"
        type="email"
        variant="outlined"
        prepend-inner-icon="mdi-email"
        :rules="emailRules"
        required
        class="mb-4"
      />
      
      <v-text-field
        v-model="form.password"
        :type="showPassword ? 'text' : 'password'"
        label="Password"
        variant="solo"
        prepend-inner-icon="mdi-lock"
        :append-inner-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
        @click:append-inner="showPassword = !showPassword"
        :rules="passwordRules"
        required
        class="mb-4"
      />

      <v-row class="mb-4">
        <v-col cols="6">
          <v-checkbox
            v-model="form.rememberMe"
            label="Ricordami"
            density="compact"
          />
        </v-col>
        <v-col cols="6" class="text-right">
          <nuxt-link 
            to="/auth/forgot-password" 
            class="text-decoration-none text-primary"
          >
            Password dimenticata?
          </nuxt-link>
        </v-col>
      </v-row>

      <v-btn
        type="submit"
        color="primary"
        size="large"
        block
        :loading="loading"
        class="mb-4"
      >
        Accedi
      </v-btn>

      <v-divider class="mb-4">
        <span class="text-medium-emphasis px-4">oppure</span>
      </v-divider>

      <v-btn
        variant="outlined"
        size="large"
        block
        prepend-icon="mdi-google"
        class="mb-4"
        @click="loginWithGoogle"
      >
        Continua con Google
      </v-btn>

      <div class="text-center">
        <span class="text-medium-emphasis">Non hai un account? </span>
        <nuxt-link 
          to="/auth/register" 
          class="text-decoration-none text-primary font-weight-medium"
        >
          Registrati
        </nuxt-link>
      </div>
    </v-form>
  </div>
</template>

<script setup lang="ts">
// Imposta il layout per questa pagina
definePageMeta({
  layout: 'auth'
})

interface LoginForm {
  email: string
  password: string
  rememberMe: boolean
}

const form = ref<LoginForm>({
  email: '',
  password: '',
  rememberMe: false
})

const showPassword = ref(false)
const loading = ref(false)

// Regole di validazione
const emailRules = [
  (v: string) => !!v || 'Email richiesta',
  (v: string) => /.+@.+\..+/.test(v) || 'Email non valida'
]

const passwordRules = [
  (v: string) => !!v || 'Password richiesta',
  (v: string) => v.length >= 6 || 'Password deve avere almeno 6 caratteri'
]

// Funzioni
const handleLogin = async () => {
  loading.value = true
  
  try {
    // Qui implementerai la logica di autenticazione
    console.log('Login con:', form.value)
    
    // Simula una chiamata API
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Redirect dopo login successo
    await navigateTo('/dashboard')
  } catch (error) {
    console.error('Errore durante il login:', error)
    // Gestisci errori qui
  } finally {
    loading.value = false
  }
}

const loginWithGoogle = () => {
  // Implementa login con Google
  console.log('Login con Google')
}
</script>