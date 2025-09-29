<template>
  <v-app>
    <!-- Navigation Drawer (Sidebar) -->
    <v-navigation-drawer
      v-model="drawer"
      :rail="rail && !mobile"
      :temporary="mobile"
      :permanent="!mobile"
      class="app-sidebar"
      :width="280"
      :rail-width="64"
    >
      <!-- Header del sidebar -->
      <v-list-item 
        class="px-2"
        :class="{ 'justify-center': rail && !mobile }"
      >
        <template v-slot:prepend>
          <v-avatar color="primary" size="32">
            <v-icon color="white">mdi-account-circle</v-icon>
          </v-avatar>
        </template>

        <v-list-item-title v-if="!rail || mobile">
          LifeSuite
        </v-list-item-title>

        <template v-slot:append v-if="!rail || mobile">
          <v-btn
            variant="text"
            icon="mdi-chevron-left"
            @click="rail = !rail"
            v-if="!mobile"
          />
        </template>
      </v-list-item>

      <v-divider />

      <!-- Menu di navigazione -->
      <v-list nav>
        <template v-for="item in menuItems" :key="item.title">
          <!-- Item con sottomenu -->
          <v-list-group v-if="item.subItems" :value="item.title">
            <template v-slot:activator="{ props }">
              <v-list-item
                v-bind="props"
                :prepend-icon="item.icon"
                :title="item.title"
                color="primary"
                class="mb-1"
              />
            </template>
            
            <v-list-item
              v-for="subItem in item.subItems"
              :key="subItem.title"
              :to="subItem.to"
              :prepend-icon="subItem.icon"
              :title="subItem.title"
              color="primary"
              class="mb-1 ml-4"
            />
          </v-list-group>
          
          <!-- Item normale -->
          <v-list-item
            v-else
            :to="item.to"
            :prepend-icon="item.icon"
            :title="item.title"
            color="primary"
            class="mb-1"
          />
        </template>
      </v-list>

      <!-- Footer del sidebar -->
      <template v-slot:append>
        <v-divider />
        <v-list nav>
          <v-list-item
            prepend-icon="mdi-cog"
            title="Impostazioni"
            @click="goToSettings"
          />
          <v-list-item
            prepend-icon="mdi-logout"
            title="Logout"
            @click="handleLogout"
          />
        </v-list>
      </template>
    </v-navigation-drawer>

    <!-- App Bar -->
    <v-app-bar
      :color="mobile ? 'primary' : 'surface'"
      elevation="2"
      height="64"
    >
      <!-- Menu button for mobile -->
      <v-app-bar-nav-icon
        v-if="mobile"
        @click="drawer = !drawer"
        color="white"
      />

      <!-- Menu toggle button for desktop -->
      <v-btn
        v-if="!mobile"
        icon="mdi-menu"
        @click="rail = !rail"
        variant="text"
      />

      <v-app-bar-title class="ml-2">
        {{ pageTitle }}
      </v-app-bar-title>

      <v-spacer />

      <!-- Right side actions -->
      <v-btn icon="mdi-bell" variant="text" />
      
      <v-menu>
        <template v-slot:activator="{ props }">
          <v-btn
            icon
            v-bind="props"
          >
            <v-avatar size="32">
              <v-icon>mdi-account-circle</v-icon>
            </v-avatar>
          </v-btn>
        </template>
        
        <v-list>
          <v-list-item>
            <v-list-item-title>{{ user?.fullName || 'Utente' }}</v-list-item-title>
            <v-list-item-subtitle>{{ user?.email || 'user@example.com' }}</v-list-item-subtitle>
          </v-list-item>
          <v-divider />
          <v-list-item @click="goToProfile">
            <v-list-item-title>Profilo</v-list-item-title>
          </v-list-item>
          <v-list-item @click="handleLogout">
            <v-list-item-title>Logout</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>
    </v-app-bar>

    <!-- Main Content -->
    <v-main>
      <v-container fluid class="pa-4">
        <slot />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { useDisplay } from 'vuetify'

const { mobile } = useDisplay()
const { user, logout } = useAuth()
const route = useRoute()

// Sidebar state
const drawer = ref(!mobile.value)
const rail = ref(false)

// Menu items
const menuItems = [
  {
    title: 'Dashboard',
    icon: 'mdi-view-dashboard',
    to: '/dashboard'
  },
  {
    title: 'Budget Manager',
    icon: 'mdi-wallet',
    subItems: [
      {
        title: 'Account',
        icon: 'mdi-bank',
        to: '/budget/accounts'
      },
      {
        title: 'Categorie',
        icon: 'mdi-tag-multiple',
        to: '/budget/categories'
      },
      {
        title: 'Transazioni',
        icon: 'mdi-swap-horizontal',
        to: '/budget/transactions'
      },
      {
        title: 'Ricorrenze',
        icon: 'mdi-repeat',
        to: '/budget/recurring'
      }
    ]
  },
  {
    title: 'Calendar',
    icon: 'mdi-calendar',
    to: '/calendar'
  },
  {
    title: 'Tasks',
    icon: 'mdi-check-circle',
    to: '/tasks'
  },
  {
    title: 'Health',
    icon: 'mdi-heart-pulse',
    to: '/health'
  },
  {
    title: 'Documents',
    icon: 'mdi-file-document',
    to: '/documents'
  }
]

// Page title computation
const pageTitle = computed(() => {
  const path = route.path
  
  // Check for budget manager subpages
  if (path.startsWith('/budget/')) {
    const budgetItem = menuItems.find(item => item.title === 'Budget Manager')
    if (budgetItem?.subItems) {
      const subItem = budgetItem.subItems.find(sub => path.startsWith(sub.to))
      if (subItem) {
        return `Budget Manager - ${subItem.title}`
      }
    }
    return 'Budget Manager'
  }
  
  // Check for regular menu items
  const currentItem = menuItems.find(item => item.to && path.startsWith(item.to))
  return currentItem?.title || 'LifeSuite'
})

// Watch mobile breakpoint changes
watch(mobile, (newValue) => {
  if (newValue) {
    rail.value = false
    drawer.value = false
  } else {
    drawer.value = true
  }
})

// Methods
const handleLogout = async () => {
  try {
    await logout()
  } catch (error) {
    console.error('Errore durante il logout:', error)
  }
}

const goToSettings = () => {
  navigateTo('/settings')
}

const goToProfile = () => {
  navigateTo('/profile')
}
</script>

<style scoped>
.v-navigation-drawer {
  border-right: 1px solid rgba(0, 0, 0, 0.12);
}

.v-app-bar {
  box-shadow: var(--nav-shadow);
}
</style>