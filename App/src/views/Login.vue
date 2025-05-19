<template>
  <div class="login-wrapper">
    <v-card class="card-login mx-auto pb-0" elevation="8" rounded="lg">
      <div class="pb-8">
        <v-img
          class="mx-auto"
          max-width="180"
          src="@/assets/Logo-AxisCRM.png"
        />
        <v-card-text>
          <v-form
            ref="form"
            v-model="valid"
            lazy-validation
            class="d-flex flex-column"
          >
            <div class="text-subtitle-1 text-medium-emphasis">E-mail</div>
            <v-text-field
              v-model="usuario.usuario"
              density="compact"
              placeholder="usuario@email"
              prepend-inner-icon="mdi-email-outline"
              variant="outlined"
              :rules="[rules.required]"
            />

            <div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
              Senha
            </div>
            <v-text-field
              v-model="usuario.senha"
              :append-inner-icon="visible ? 'mdi-eye-off' : 'mdi-eye'"
              :type="visible ? 'text' : 'password'"
              :readonly="loading"
              density="compact"
              placeholder="Senha"
              prepend-inner-icon="mdi-lock-outline"
              variant="outlined"
              :rules="[rules.required]"
              @click:append-inner="visible = !visible"
            />
          </v-form>
        </v-card-text>

        <v-card-actions class="pt-4">
          <v-btn
            :disabled="loading"
            :loading="loading"
            color="primary"
            type="button"
            variant="elevated"
            @click="handleLogin"
            block
          >
            <template v-if="loading">
              <v-progress-circular indeterminate size="20" />
            </template>
            <template v-else> Entrar </template>
          </v-btn>
        </v-card-actions>
      </div>
    </v-card>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  name: "TelaLogin",
  data() {
    return {
      usuario: {
        usuario: "",
        senha: "",
        manterConectado: false,
      },
      valid: false,
      loading: false,
      visible: false,
      rules: {
        required: (v) => !!v || "Campo obrigatório",
      },
    };
  },
  mounted() {
    window.addEventListener("keydown", this.handleEnterKey);
  },
  beforeUnmount() {
    window.removeEventListener("keydown", this.handleEnterKey);
  },
  methods: {
    ...mapActions("auth", ["login"]),

    handleEnterKey(e) {
      if (e.key === "Enter" && this.valid && !this.loading) {
        this.handleLogin();
      }
    },

    async handleLogin() {
      if (!this.valid) {
        this.$refs.form.validate();
        return;
      }

      this.loading = true;
      try {
        await this.login({
          email: this.usuario.usuario,
          senha: this.usuario.senha,
          manterConectado: this.usuario.manterConectado,
        });

        this.$router.push("/dashboard");
      } catch (err) {
        this.$refs.form.resetValidation();
        this.$emit("error", "Usuário ou senha inválidos");
      } finally {
        this.loading = false;
      }
    },
  },
};
</script>

<style scoped>
.v-main {
  height: 100vh;
  background: linear-gradient(to bottom right, #1976d2 60%, black 100%);
}
.login-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 90vh;
}
.card-login {
  width: 390px;
  height: 490px;
  padding-left: 48px;
  padding-right: 48px;
}
</style>
