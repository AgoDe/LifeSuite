<template>
  <v-app>
    <v-main>
      <v-container class="fill-height" fluid>
        <v-row class="fill-height" no-gutters>
          <!-- Sezione immagine/branding -->
          <v-col cols="12" md="6" class="auth-bg d-none d-md-flex">
            <v-container class="fill-height d-flex align-center justify-center">
              <div class="text-center">
                <v-icon size="120" color="white">mdi-account-circle</v-icon>
                <h1 class="mt-4 text-h3 font-weight-bold text-white">LifeSuite</h1>
                <p class="mt-2 text-h6 text-white">La tua vita, organizzata</p>
              </div>
            </v-container>
          </v-col>
          
          <!-- Sezione form -->
          <v-col cols="12" md="6" class="d-flex align-center justify-center">
            <v-container style="max-width: 400px;">
                <slot />
            </v-container>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
.auth-bg {
  background: var(--auth-gradient, linear-gradient(135deg, #667eea 0%, #764ba2 100%));
}
</style>