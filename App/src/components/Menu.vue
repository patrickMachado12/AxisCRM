<template>
  <div>
    <v-app-bar color="#1976D2" elevation="2">
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-toolbar-title class="logo-menu">
        <v-img src="@/assets/Logo-menu.png" height="160" contain />
      </v-toolbar-title>
      <v-btn icon @click="toggleDark">
        <v-icon>{{ isDark ? 'mdi-white-balance-sunny' : 'mdi-moon-waning-crescent' }}</v-icon>
      </v-btn>
      <v-btn prepend-icon="mdi-exit-to-app" @click="logOff" variant="text" size="large">
        Sair
      </v-btn>
    </v-app-bar>
    <v-navigation-drawer v-model="drawer" temporary>
      <v-list nav dense>
        <v-list-group value="CRM">
          <template #activator="{ props }">
            <v-list-item
              v-bind="props"
              prepend-icon="mdi-account-circle"
              variant="plain">            
            CRM
            </v-list-item>
          </template>
          <v-list-item 
            @click="navigate('Administrador')" 
            :active="isRoute('Administrador')" 
            prepend-icon="mdi mdi-card-account-details-outline"
            variant="plain">
            Administrador
          </v-list-item>
          <v-list-item 
            @click="navigate('Atendimento')" 
            :active="isRoute('Atendimento')" 
            prepend-icon="mdi mdi-account-details"
            variant="plain">
            Atendimento
          </v-list-item>
        </v-list-group>
        <v-list-item 
          @click="navigate('ControleCliente')" 
          :active="isRoute('ControleCliente')"
          prepend-icon="mdi mdi-account-group"
          variant="plain">
          Clientes
        </v-list-item>
        <v-list-item 
          @click="navigate('ControleUsuario')" 
          :active="isRoute('ControleUsuario')"
          prepend-icon="mdi mdi-account"
          variant="plain">
          Usuários
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
  </div>
</template>

<script>
import { defineComponent, ref, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useTheme } from 'vuetify';

export default defineComponent({
  name: 'MenuGeral',
  setup() {
    const drawer = ref(false);
    const router = useRouter();
    const route = useRoute();
    const theme = useTheme();
    const isDark = computed(() => theme.global.current.value.dark);
    function toggleDark() {
      theme.global.name.value = isDark.value ? 'light' : 'dark';
    }

    function navigate(routeName) {
      if (route.name !== routeName) {
        router.push({ name: routeName });
      }
      drawer.value = false;
    }

    function isRoute(routeName) {
      return route.name === routeName;
    }

    function logOff() {
      localStorage.clear();
      if (route.name !== 'Login') {
        router.push({ name: 'Login' });
      }
    }

    return {
      drawer,
      isDark,
      toggleDark,
      navigate,
      isRoute,
      logOff,
    };
  },
});
</script>

<style scoped>
.v-list-item-icon .v-icon {
  font-size: 32px;
}

.logo-menu {
  margin-inline-start: 0px;
  margin-top: 10px;
}
</style>
