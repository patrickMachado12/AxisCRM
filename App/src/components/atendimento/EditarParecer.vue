<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card>
      <v-card-title>Editar Parecer</v-card-title>
      <v-card-text>
        <v-textarea
          v-model="descricao"
          label="Descrição"
          rows="3"
          auto-grow
          outlined
          :rules="[rules.required]"
        />
        <v-text-field
          v-model="pessoaContato"
          label="Pessoa Contato"
          dense
          outlined
          :rules="[rules.required]"
        />
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
            class="mr-2"
            color="primary"
            @click="submit"
            rounded="3"
            variant="flat"
            :disabled="!isValid"
          >
            Gravar
          </v-btn>
          <v-btn class="mr-2" variant="tonal" @click="$emit('cancel')">
            Cancelar
          </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { ref, watch, computed } from "vue";
import parecerService from "@/services/parecer-service";

export default {
  name: "EditarParecer",
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    atendimentoId: {
      type: [String, Number],
      required: true,
    },
    parecer: {
      type: Object,
      required: true,
    },
  },
  emits: ["update:modelValue", "submitted"],
  setup(props, { emit }) {
    const dialog = ref(props.modelValue);
    const descricao = ref(props.parecer.descricao || "");
    const pessoaContato = ref(props.parecer.pessoaContato || "");

    const rules = {
      required: (v) => !!v || "Campo obrigatório",
    };

    const isValid = computed(() => {
      return (
        descricao.value.trim().length > 0 &&
        pessoaContato.value.trim().length > 0
      );
    });

    watch(
      () => props.modelValue,
      (val) => (dialog.value = val)
    );
    watch(dialog, (val) => emit("update:modelValue", val));

    watch(
      () => props.parecer,
      (p) => {
        descricao.value = p.descricao || "";
        pessoaContato.value = p.pessoaContato || "";
      }
    );

    const close = () => {
      descricao.value = props.parecer.descricao || "";
      pessoaContato.value = props.parecer.pessoaContato || "";
      dialog.value = false;
    };

    const submit = async () => {
      try {
        await parecerService.atualizar(props.atendimentoId, props.parecer.id, {
          descricao: descricao.value.trim(),
          pessoaContato: pessoaContato.value.trim(),
        });
        emit("submitted");
        dialog.value = false;
      } catch (err) {
        console.error("Erro ao editar parecer:", err);
      }
    };

    return {
      dialog,
      descricao,
      pessoaContato,
      rules,
      isValid,
      close,
      submit,
    };
  },
};
</script>

<style scoped>
</style>
