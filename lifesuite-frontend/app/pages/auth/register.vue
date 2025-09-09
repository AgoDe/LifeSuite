<template>
  <div>
    <div class="text-center mb-8">
      <h1 class="text-h4 font-weight-bold mb-2">Registrati</h1>
      <p class="text-body-1 text-medium-emphasis">Crea il tuo account LifeSuite</p>
    </div>

    <v-form @submit.prevent="handleRegister" ref="form">
      <v-row>
        <v-col cols="12" sm="6">
          <v-text-field
            v-model="form.firstName"
            label="Nome"
            type="text"
            variant="outlined"
            prepend-inner-icon="mdi-account"
            :rules="nameRules"
            required
            class="mb-4"
          />
        </v-col>
        <v-col cols="12" sm="6">
          <v-text-field
            v-model="form.lastName"
            label="Cognome"
            type="text"
            variant="outlined"
            prepend-inner-icon="mdi-account"
            :rules="nameRules"
            required
            class="mb-4"
          />
        </v-col>
      </v-row>

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
        variant="outlined"
        prepend-inner-icon="mdi-lock"
        :append-inner-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
        @click:append-inner="showPassword = !showPassword"
        :rules="passwordRules"
        required
        class="mb-4"
      />

      <v-text-field
        v-model="form.confirmPassword"
        :type="showConfirmPassword ? 'text' : 'password'"
        label="Conferma Password"
        variant="outlined"
        prepend-inner-icon="mdi-lock-check"
        :append-inner-icon="showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'"
        @click:append-inner="showConfirmPassword = !showConfirmPassword"
        :rules="confirmPasswordRules"
        required
        class="mb-4"
      />

      <v-checkbox
        v-model="form.acceptTerms"
        :rules="termsRules"
        required
        class="mb-4"
      >
        <template v-slot:label>
          <span class="text-body-2">
            Accetto i 
            <a href="/terms" target="_blank" class="text-primary text-decoration-none">
              Termini di Servizio
            </a>
            e la 
            <a href="/privacy" target="_blank" class="text-primary text-decoration-none">
              Privacy Policy
            </a>
          </span>
        </template>
      </v-checkbox>

      <v-btn
        type="submit"
        color="primary"
        size="large"
        block
        :loading="loading"
        class="mb-4"
      >
        Registrati
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
        @click="registerWithGoogle"
      >
        Continua con Google
      </v-btn>

      <div class="text-center">
        <span class="text-medium-emphasis">Hai già un account? </span>
        <nuxt-link 
          to="/auth/login" 
          class="text-decoration-none text-primary font-weight-medium"
        >
          Accedi
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

interface RegisterForm {
  firstName: string
  lastName: string
  email: string
  password: string
  confirmPassword: string
  acceptTerms: boolean
}

const form = ref<RegisterForm>({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: '',
  acceptTerms: false
})

const showPassword = ref(false)
const showConfirmPassword = ref(false)
const loading = ref(false)

// Regole di validazione
const nameRules = [
  (v: string) => !!v || 'Campo richiesto',
  (v: string) => v.length >= 2 || 'Minimo 2 caratteri'
]

const emailRules = [
  (v: string) => !!v || 'Email richiesta',
  (v: string) => /.+@.+\..+/.test(v) || 'Email non valida'
]

const passwordRules = [
  (v: string) => !!v || 'Password richiesta',
  (v: string) => v.length >= 8 || 'Password deve avere almeno 8 caratteri',
  (v: string) => /[A-Z]/.test(v) || 'Password deve contenere almeno una lettera maiuscola',
  (v: string) => /[a-z]/.test(v) || 'Password deve contenere almeno una lettera minuscola',
  (v: string) => /[0-9]/.test(v) || 'Password deve contenere almeno un numero'
]

const confirmPasswordRules = [
  (v: string) => !!v || 'Conferma password richiesta',
  (v: string) => v === form.value.password || 'Le password non corrispondono'
]

const termsRules = [
  (v: boolean) => !!v || 'Devi accettare i termini di servizio'
]

// Funzioni
const handleRegister = async () => {
  loading.value = true
  
  try {
    // Qui implementerai la logica di registrazione
    console.log('Registrazione con:', form.value)
    
    // Simula una chiamata API
    await new Promise(resolve => setTimeout(resolve, 1500))
    
    // Redirect dopo registrazione successo
    await navigateTo('/auth/login?registered=true')
  } catch (error) {
    console.error('Errore durante la registrazione:', error)
    // Gestisci errori qui
  } finally {
    loading.value = false
  }
}

const registerWithGoogle = () => {
  // Implementa registrazione con Google
  console.log('Registrazione con Google')
}
</script>