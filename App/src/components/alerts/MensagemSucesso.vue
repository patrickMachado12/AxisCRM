<template>
  <v-snackbar v-model="visible" :timeout="timeout" top right color="success">
    {{ message }}
    <v-btn text @click="visible = false">
      <v-icon>mdi-close</v-icon>
    </v-btn>
  </v-snackbar>
</template>

<script>
export default {
  name: 'MensagemSucesso',
  props: {
    message: {
      type: String,
      required: true,
    },
    timeout: {
      type: Number,
      default: 4000,
    },
  },
  data() {
    return {
      visible: false,
    };
  },
  methods: {
    show() {
      this.visible = true;
    },
    hide() {
      this.visible = false;
    },
  },
};
</script>
