<template>
  <v-dialog v-model="showParecer" max-width="500" persistent>
    <v-form ref="formRef" @submit.prevent="onSubmit">
      <v-card style="overflow-y: hidden">
        <v-card-title>Parecer</v-card-title>
        <v-divider />
        <v-card-text style="overflow-y: auto">
          <v-row>
            <v-col cols="12" md="4">
              <v-text-field
                v-model="atendimento.id"
                label="Atendimento"
                readonly
                denses
              />
            </v-col>
            <v-col cols="12" md="8">
              <v-text-field
                v-model="atendimento.assunto"
                label="Assunto"
                readonly
                dense
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <v-textarea
                v-model="descricao"
                label="Descrição do parecer"
                rows="6"
                no-resize
                outlined
                dense
                :rules="[rules.required]"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="pessoaContato"
                label="Pessoa de contato"
                outlined
                dense
                :rules="[rules.required]"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <v-radio-group
                v-model="status"
                row
                dense
                :rules="[rules.required]"
              >
                <v-radio
                  v-for="opt in statusOptions"
                  :key="opt.value"
                  :label="opt.label"
                  :value="opt.value"
                />
              </v-radio-group>
            </v-col>
          </v-row>
        </v-card-text>
        <v-divider />
        <v-card-actions>
          <v-btn
            class="mr-2"
            color="primary"
            @click="onSubmit"
            rounded="3"
            variant="flat"
          >
            Gravar
          </v-btn>

          <v-btn class="mr-2" variant="tonal" @click="$emit('cancel')">
            Cancelar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script>
import { ref, reactive, toRefs, onMounted } from "vue";
import atendimentoService from "@/services/atendimento-service";
import * as parecerService from "@/services/parecer-service";

export default {
  name: "NovoParecer",
  emits: ["saved", "cancel"],
  props: {
    atendimentoId: {
      type: [String, Number],
      required: true,
    },
  },
  setup(props, { emit }) {
    const formRef = ref(null);

    const atendimento = reactive({
      id: props.atendimentoId,
      assunto: "",
    });

    const form = reactive({
      descricao: "",
      pessoaContato: "",
    });

    const rules = {
      required: (v) => !!v || "Campo obrigatório",
    };

    const statusOptions = [
      { label: "Aberto", value: "Aberto" },
      { label: "Encerrado", value: "Encerrado" },
    ];

    const status = ref("Aberto");

    const statusMap = {
      1: "Aberto",
      2: "Encerrado",
    };

    onMounted(async () => {
      try {
        const data = await atendimentoService.obterAtendimentoPorId(
          props.atendimentoId
        );
        atendimento.assunto = data.assunto;
        status.value = statusMap[data.status] || "Aberto";

        if (Array.isArray(data.pareceres) && data.pareceres.length > 0) {
          const ultimoParecer = data.pareceres[data.pareceres.length - 1];
          form.pessoaContato = ultimoParecer.pessoaContato || "";
        }
      } catch (e) {
        console.error("Erro ao carregar atendimento:", e);
      }
    });

    async function onSubmit() {
      const valid = await formRef.value.validate();
      if (!valid) return;

      const payload = {
        descricao: form.descricao,
        pessoaContato: form.pessoaContato,
        status: status.value,
      };

      try {
        await parecerService.cadastrar(atendimento.id, payload);
        emit("saved");
      } catch (e) {
        console.error("Erro ao gravar parecer:", e);
      }
    }

    return {
      formRef,
      status,
      statusOptions,
      atendimento,
      ...toRefs(form),
      rules,
      onSubmit,
    };
  },
};
</script>

<style scoped></style>
