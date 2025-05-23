import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import pt from 'vuetify/lib/locale/pt.mjs'

export default createVuetify({
  components,
  directives,
  locale: {
    locale: 'pt',
    fallback: 'en',
    messages: { pt },
  },
  theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        dark: false,
        colors: {
          primary:   '#1976D2',
          secondary: '#424242', 
          success:   '#4CAF50', 
          error:     '#F44336', 
          warning:   '#FB8C00', 
          info:      '#2196F3', 
          background:'#FFFFFF',
          surface:   '#FFFFFF',
        },
      },
      dark: {
        dark: true,
        colors: {
          primary:   '#1976D2',
          secondary: '#424242',
          success:   '#4CAF50',
          error:     '#F44336',
          warning:   '#FB8C00',
          info:      '#2196F3',
          background:'#121212',
          surface:   '#1E1E1E',
        },
      },
    },
  },
})
