<template>
  <v-dialog v-model="showLog" max-width="600" persistent>
    <v-card class="historico-card d-flex flex-column" style="height: 100%">
      <v-card-title class="d-flex justify-space-between align-center">
        <span>Auditoria Atendimento #{{ atendimentoId }}</span>
      </v-card-title>
      <v-divider />
      <v-card-text class="flex-grow-1 overflow-y-auto pa-4">
        <v-skeleton-loader v-if="loading" type="card" class="mb-4" />
        <v-alert v-else-if="error" type="error" dense>
          {{ error }}
        </v-alert>
        <div v-else>
          <v-card
            v-for="(entry, idx) in entries"
            :key="idx"
            class="mb-4"
            outlined
            elevation="3"
          >
            <v-card-text>
              {{ entry }}
            </v-card-text>
          </v-card>

          <div
            v-if="!entries.length && !loading"
            class="text-center grey--text"
          >
            <em>Nenhum log de alterações registrado.</em>
          </div>
        </div>
      </v-card-text>
      <v-card-actions>
        <v-btn class="mr-2" variant="tonal" @click="$emit('close')">
          Fechar
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { ref, onMounted } from "vue";
import atendimentoService from "@/services/atendimento-service";

export default {
  name: "LogsAtendimento",
  emits: ["close"],
  props: {
    atendimentoId: {
      type: [String, Number],
      required: true,
    },
  },
  setup(props, { emit }) {
    const loading = ref(false);
    const error = ref("");
    const historico = ref("");

    const entries = ref([]);

    onMounted(async () => {
      loading.value = true;
      try {
        const data = await atendimentoService.obterAtendimentoPorId(
          props.atendimentoId
        );
        historico.value = data.historico || "";
        entries.value = historico.value
          .split(/\r?\n/)
          .map((l) => l.trim())
          .filter((l) => l.length);
      } catch (e) {
        console.error("Erro ao carregar logs:", e);
        error.value = "Não foi possível carregar os logs de alterações.";
      } finally {
        loading.value = false;
      }
    });

    return {
      loading,
      error,
      entries,
    };
  },
};
</script>

<style scoped></style>
