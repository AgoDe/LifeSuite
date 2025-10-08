<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title>Test API Gateway Connection</v-card-title>
          <v-card-text>
            <div class="d-flex flex-wrap ga-2 mb-4">
              <v-btn @click="testHealth" color="primary" size="small">
                Health Check
              </v-btn>
              <v-btn @click="testCors" color="secondary" size="small">
                Test CORS
              </v-btn>
              <v-btn @click="testAuth" color="success" size="small" :disabled="!isLoggedIn">
                Test Auth
              </v-btn>
              <v-btn @click="clearResults" color="error" size="small" variant="outlined">
                Clear
              </v-btn>
            </div>
            
            <v-alert v-if="result" :type="result.type" class="mb-4">
              <strong>{{ result.title }}</strong><br>
              {{ result.message }}
            </v-alert>
            
            <v-card v-if="result?.data" variant="outlined">
              <v-card-text>
                <pre style="font-size: 12px; overflow-x: auto;">{{ JSON.stringify(result.data, null, 2) }}</pre>
              </v-card-text>
            </v-card>
            
            <v-divider class="my-4"></v-divider>
            
            <div>
              <p><strong>Status:</strong> {{ isLoggedIn ? 'Logged In' : 'Not Logged In' }}</p>
              <p><strong>API URL:</strong> {{ apiBaseUrl }}</p>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
const { apiCall, baseURL } = useApi()
const { isLoggedIn } = useAuth()

const result = ref<any>(null)
const apiBaseUrl = baseURL

const testHealth = async () => {
  try {
    const response = await fetch(`${baseURL}/api/health`, {
      method: 'GET',
      credentials: 'include'
    })
    
    if (response.ok) {
      const data = await response.json()
      result.value = {
        type: 'success',
        title: 'Health Check Success',
        message: 'API Gateway è online e risponde!',
        data
      }
    } else {
      throw new Error(`HTTP ${response.status}`)
    }
  } catch (error: any) {
    result.value = {
      type: 'error',
      title: 'Health Check Failed',
      message: `Errore: ${error.message}`
    }
  }
}

const testCors = async () => {
  try {
    const response = await apiCall('test/cors', {
      method: 'POST',
      body: { test: 'CORS test data' }
    })
    
    result.value = {
      type: 'success',
      title: 'CORS Test Success',
      message: 'CORS configurato correttamente con preflight!',
      data: response
    }
  } catch (error: any) {
    result.value = {
      type: 'error',
      title: 'CORS Test Failed',
      message: `CORS Error: ${error.message}`
    }
  }
}

const testAuth = async () => {
  try {
    const response = await apiCall('test/auth')
    result.value = {
      type: 'success',
      title: 'Auth + CORS Success',
      message: 'Autenticazione e CORS funzionano insieme!',
      data: response
    }
  } catch (error: any) {
    result.value = {
      type: 'error',
      title: 'Auth Test Failed',
      message: 'Fai login prima di testare l\'auth'
    }
  }
}

const clearResults = () => {
  result.value = null
}
</script>