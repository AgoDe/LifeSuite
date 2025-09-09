<template>
  <div>
    <!-- Loading state mentre controlla l'autenticazione -->
    <div class="d-flex justify-center align-center fill-height">
      <v-progress-circular indeterminate size="64" />
    </div>
  </div>
</template>

<script setup lang="ts">
// Controlla se l'utente è autenticato e reindirizza di conseguenza
const { checkAuth } = useAuth()

try {
  const isAuthenticated = await checkAuth()
  
  if (isAuthenticated) {
    await navigateTo('/dashboard')
  } else {
    await navigateTo('/auth/login')
  }
} catch (error) {
  console.error('Errore nel controllo autenticazione:', error)
  await navigateTo('/auth/login')
}
</script>
