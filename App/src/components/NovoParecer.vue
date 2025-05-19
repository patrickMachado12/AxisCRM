<!-- src/components/ParecerForm.vue -->
<template>
  <v-form ref="formRef" @submit.prevent="onSubmit">
    <!-- Exibe o ID do atendimento -->
    <v-row>
      <v-col cols="12" md="4">
        <v-text-field
          v-model="atendimento.id"
          label="Atendimento"
          readonly
          dense
        />
      </v-col>
      <!-- Exibe o assunto original -->
      <v-col cols="12" md="8">
        <v-text-field
          v-model="atendimento.assunto"
          label="Assunto"
          readonly
          dense
        />
      </v-col>
    </v-row>

    <!-- Texto do parecer -->
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

    <!-- Pessoa de contato no parecer -->
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

    <!-- Ações -->
    <v-row justify="end" class="mt-4">
      <v-btn text @click="$emit('cancel')">Cancelar</v-btn>
      <v-btn color="primary" @click="onSubmit">Gravar Parecer</v-btn>
    </v-row>
  </v-form>
</template>

<script>

import { ref, reactive, toRefs, onMounted } from 'vue';
import atendimentoService from '@/services/atendimento-service';
import * as parecerService from '@/services/parecer-service'; // importa seu service



export default {
  // name: 'NovoParecer',
  // emits: ['saved', 'cancel'],
  // props: {
  //   /** ID do atendimento que vamos preencher o parecer */
  //   atendimentoId: {
  //     type: [String, Number],
  //     required: true
  //   }
  // },

  name: 'NovoParecer',
  emits: ['saved','cancel'],
  props: {
    /** recebe apenas o id do atendimento para o qual faremos o parecer */
    atendimentoId: {
      type: [String, Number],
      required: true
    }
  },
  setup(props, { emit }) {
    const formRef = ref(null);

    // dados do atendimento
    const atendimento = reactive({
      id: props.atendimentoId,
      assunto: ''
    });

    // estado do form de parecer
    const form = reactive({
      descricao: '',
      pessoaContato: ''
    });

    const rules = {
      required: v => !!v || 'Campo obrigatório'
    };

    // ao montar, busca o assunto do atendimento
    onMounted(async () => {
      try {
        const data = await atendimentoService.obterAtendimentoPorId(props.atendimentoId);
        atendimento.assunto = data.assunto;
      } catch (e) {
        console.error('Erro ao carregar atendimento:', e);
      }
    });

    // submete o parecer
    async function onSubmit() {
      const valid = await formRef.value.validate();
      if (!valid) return;

      try {
        // chama seu endpoint: POST /atendimentos/{idAtendimento}/pareceres
        await parecerService.cadastrar(
          atendimento.id,
          {
            descricao: form.descricao,
            pessoaContato: form.pessoaContato
          }
        );
        emit('saved');
      } catch (e) {
        console.error('Erro ao gravar parecer:', e);
      }
    }

    return {
      formRef,
      atendimento,
      ...toRefs(form),
      rules,
      onSubmit
    };
  }
};
</script>

<style scoped>
/* ajustes finos, se necessário */
</style>
