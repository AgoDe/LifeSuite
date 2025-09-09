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
const { isLoggedIn } = useAuth()

if (isLoggedIn.value) {
  await navigateTo('/dashboard')
} else {
  await navigateTo('/auth/login')
}
</script>
