<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card>
      <v-card-title>Motivo da Reabertura</v-card-title>
      <v-divider />
      <v-card-text>
        <v-textarea
          v-model="motivo"
          label="Motivo"
          rows="3"
          auto-grow
          outlined
        />
      </v-card-text>
      <v-divider />
      <v-card-actions>
        <v-spacer />
        <v-btn
          class="mr-2"
          color="primary"
          @click="submit"
          rounded="3"
          variant="flat"
        >
          Reabrir
        </v-btn>
        <v-btn class="mr-2" variant="tonal" @click="close"> Cancelar </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { ref, watch } from "vue";
import atendimentoService from "@/services/atendimento-service";

export default {
  name: "MotivoReabertura",
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    atendimento: {
      type: Object,
      required: true,
    },
  },
  emits: ["update:modelValue", "submitted"],
  setup(props, { emit }) {
    const dialog = ref(props.modelValue);
    const motivo = ref("");

    watch(
      () => props.modelValue,
      (v) => (dialog.value = v)
    );
    watch(dialog, (v) => emit("update:modelValue", v));

    const close = () => {
      motivo.value = "";
      dialog.value = false;
    };

    const submit = async () => {
      try {
        await atendimentoService.alterarStatusAtendimento(
          props.atendimento.id,
          {
            status: "Reaberto",
            motivo: motivo.value.trim(),
          }
        );
        emit("submitted");
        close();
      } catch (err) {
        console.error(err);
      }
    };

    return { dialog, motivo, close, submit };
  },
};
</script>

<style scoped>
</style>
