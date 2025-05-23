<template>
  <v-app>
    <Menu v-if="$route.name !== 'Login'" />
    <v-main>
      <v-container fluid>
        <router-view></router-view>
      </v-container>
    </v-main>
  </v-app>
</template>
<script>
import Menu from "@/components/Menu.vue";

export default {
  name: "App",

  components: {
    Menu,
  },

  data: () => ({}),
};
</script>

<style>
@import url("https://fonts.googleapis.com/css2?family=Quicksand:wght@300;400;500;600;700&display=swap");

html {
  overflow-y: auto !important;
  overflow-x: auto !important;
}

body {
  margin: 0;
  padding: 0;
  font-family: "Quicksand", sans-serif;
}

</style>
