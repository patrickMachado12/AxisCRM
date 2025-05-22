<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card>
      <v-card-title>Editar Atendimento</v-card-title>
      <v-card-text>
        <v-text-field
          v-model="idCliente"
          label="Código do Cliente"
          type="number"
          dense
          outlined
          :rules="[rules.required]"
        />
        <v-text-field
          v-model="assunto"
          label="Assunto"
          dense
          outlined
          :rules="[rules.required]"
        />
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
          color="primary"
          @click="submit"
          :disabled="!isValid"
        >
          Salvar
        </v-btn>
        <v-btn text @click="close">Cancelar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { ref, watch, computed } from "vue";
import atendimentoService from "@/services/atendimento-service";

export default {
  name: "EditarAtendimento",
  props: {
    modelValue: {
      type: Boolean,
      required: true
    },
    atendimento: {
      type: Object,
      required: true
    }
  },
  emits: ["update:modelValue", "submitted"],
  setup(props, { emit }) {
    const dialog = ref(props.modelValue);
    const idCliente = ref(props.atendimento.idCliente);
    const assunto   = ref(props.atendimento.assunto);

    const rules = {
      required: v => !!v || "Campo obrigatório"
    };
    const isValid = computed(
      () => !!idCliente.value && assunto.value.trim().length > 0
    );

    watch(() => props.modelValue, v => (dialog.value = v));
    watch(dialog, v => emit("update:modelValue", v));

    watch(() => props.atendimento, at => {
      idCliente.value = at.idCliente;
      assunto.value   = at.assunto;
    });

    const close = () => {
      idCliente.value = props.atendimento.idCliente;
      assunto.value = props.atendimento.assunto;
      dialog.value = false;
    };

    const submit = async () => {
      try {
        await atendimentoService.atualizarAtendimento({
          id: props.atendimento.id,
          idCliente: Number(idCliente.value),
          assunto: assunto.value.trim()
        });
        emit("submitted");
        dialog.value = false;
      } catch (err) {
        console.error("Erro ao editar atendimento:", err);
      }
    };

    return {
      dialog,
      idCliente,
      assunto,
      rules,
      isValid,
      close,
      submit
    };
  }
};
</script>

<style scoped>
</style>
