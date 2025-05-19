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
  // theme: {
  //   themes: {
  //     light: {
  //       colors: {
  //         primary: '#1976D2',
  //         secondary: '#5CBBF6',
  //       },
  //     },
  //   },
  // },

  theme: {
    defaultTheme: "light",    // tema inicial
    themes: {
      light: {
        dark: false,
        colors: {
          primary: "#1976D2",
          secondary: "#424242",
          background: "#FFFFFF",
          surface: "#FFFFFF",
          // defina suas cores customizadas...
        },
      },
      dark: {
        dark: true,
        colors: {
          primary: "#2196F3",
          secondary: "#B0BEC5",
          background: "#121212",
          surface: "#1E1E1E",
          // ajuste as cores para o modo dark
        },
      },
    },
  },
})
