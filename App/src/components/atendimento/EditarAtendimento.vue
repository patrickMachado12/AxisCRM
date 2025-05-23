<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card style="overflow-y: hidden">
      <v-card-title>Editar Atendimento</v-card-title>
      <v-divider />
      <v-card-text>
        <v-col cols="12" md="4">
          <v-text-field
            v-model="idCliente"
            label="Código Cliente"
            type="text"
            outlined
            dense
            :rules="[rules.required]"
          />
        </v-col>
        <v-col cols="12" md="12">
          <v-text-field
          v-model="assunto"
          label="Assunto"
          dense
          outlined
          :rules="[rules.required]"
          />
        </v-col>
      </v-card-text>
      <v-divider/>
      <v-card-actions>
        <v-btn
          color="primary"
          @click="submit"
          :disabled="!isValid"
          class="mr-2"
          rounded="3"
          variant="flat"
        >
          Gravar
        </v-btn>
        <v-btn 
          class="mr-2"
          variant="tonal" 
          @click="close"
        >
          Cancelar
        </v-btn>
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
