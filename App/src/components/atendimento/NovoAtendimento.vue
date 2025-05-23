<template>
  <v-dialog v-model="showForm" max-width="700" scrollable persistent>
    <v-form ref="formRef" @submit.prevent="onSubmit">
      <v-card style="overflow-y: hidden">
        <v-card-title>Novo atendimento</v-card-title>
        <v-divider />
        <v-card-text>
          <v-row>
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
          </v-row>
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="assunto"
                label="Assunto"
                :maxlength="255"
                :rules="[rules.required]"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <v-textarea
                v-model="descricao"
                label="Descrição do parecer"
                rows="10"
                no-resize
                outlined
                dense
                :rules="[rules.required]"
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="pessoaContato"
                label="Pessoa de contato"
                outlined
                dense
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <label class="d-block mb-2">Status</label>
              <v-radio-group v-model="status" row>
                <v-radio label="Aberto" :value="1" />
                <v-radio label="Encerrado" :value="2" />
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
import { ref, reactive, toRefs } from "vue";

export default {
  name: "NovoAtendimentoForm",
  emits: ["save", "cancel"],
  props: {
    initial: {
      type: Object,
      default: () => ({}),
    },
  },
  setup(props, { emit }) {
    const formRef = ref(null);

    const form = reactive({
      idCliente: props.initial.idCliente || "",
      status: props.initial.status || 1,
      assunto: props.initial.assunto || "",
      descricao: props.initial.parecer?.descricao || "",
      pessoaContato: props.initial.parecer?.pessoaContato || "",
    });

    const rules = {
      required: (v) => !!v || "Campo obrigatório",
    };

    function onSubmit() {
      formRef.value.validate().then((ok) => {
        if (ok) {
          const payload = {
            assunto: form.assunto,
            idCliente: Number(form.idCliente),
            status: form.status,
            parecer: {
              descricao: form.descricao,
              pessoaContato: form.pessoaContato,
            },
          };
          emit("save", payload);
        }
      });
    }

    return {
      formRef,
      ...toRefs(form),
      rules,
      onSubmit,
    };
  },
};
</script>

<style scoped>
</style>
