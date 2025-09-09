// plugins/vuetify.ts
import { defineNuxtPlugin } from 'nuxt/app';
import { createVuetify } from 'vuetify';
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';

export default defineNuxtPlugin((nuxtApp) => {
  const vuetify = createVuetify({
    components,
    directives,
    theme: {
      defaultTheme: 'lifeSuite',
      themes: {
        lifeSuite: {
          colors: {
            primary: '#667eea',
            'primary-darken-1': '#5a6fd8',
            secondary: '#764ba2',
            'secondary-darken-1': '#6a4190',
            accent: '#ff6b6b',
            error: '#f44336',
            info: '#2196f3',
            success: '#4caf50',
            warning: '#ff9800',
            background: '#fafafa',
            surface: '#ffffff',
            'on-primary': '#ffffff',
            'on-secondary': '#ffffff',
            'on-background': '#212121',
            'on-surface': '#212121',
          }
        }
      }
    },
    defaults: {
      VBtn: {
        style: 'text-transform: none;',
      },
      VCard: {
        elevation: 2,
      }
    }
  });
  
  nuxtApp.vueApp.use(vuetify);
});
