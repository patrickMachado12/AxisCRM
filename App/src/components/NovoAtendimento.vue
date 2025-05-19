<template>
  <v-form ref="formRef" @submit.prevent="onSubmit">
    <v-row>
      <v-col cols="12" md="4">
        <v-text-field
          v-model="idCliente"
          label="Código do Cliente"
          type="number"
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
          outlined
          dense
          :rules="[rules.required]"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-textarea
          v-model="descricao"
          label="Descrição do parecer"
          rows="4"
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

    <v-row justify="end" class="mt-4">
      <v-btn text @click="$emit('cancel')">Cancelar</v-btn>
      <v-btn color="primary" @click="onSubmit">Salvar</v-btn>
    </v-row>
  </v-form>
</template>

<script>
import { ref, reactive, toRefs } from 'vue';

export default {
  name: 'NovoAtendimentoForm',
  emits: ['save', 'cancel'],
  props: {
    initial: {
      type: Object,
      default: () => ({})
    }
  },
  setup(props, { emit }) {
    const formRef = ref(null);

    const form = reactive({
      idCliente : props.initial.idCliente || '',
      status : props.initial.status || 1,
      assunto : props.initial.assunto || '',
      descricao : props.initial.parecer?.descricao || '',
      pessoaContato : props.initial.parecer?.pessoaContato || ''
    });

    const rules = {
      required: v => !!v || 'Campo obrigatório'
    };

    function onSubmit() {
      formRef.value.validate().then(ok => {
        if (ok) {
          const payload = {
            assunto: form.assunto,
            idCliente: Number(form.idCliente),
            status: form.status,
            parecer: {
              descricao: form.descricao,
              pessoaContato: form.pessoaContato
            }
          };
          emit('save', payload);
        }
      });
    }

    return {
      formRef,
      ...toRefs(form),
      rules,
      onSubmit
    };
  }
};
</script>

<style scoped>

</style>
