<template>
  <v-app>
    <Menu />
    <v-main>
      <v-container fluid>
        <slot/>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
import Menu from "@/components/Menu.vue";
export default {
  name: "DefaultLayout",
  components: { Menu }
}
</script>
