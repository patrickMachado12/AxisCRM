<template>
  <v-card>
    <!-- Cabeçalho com dados do atendimento -->
    <v-card-title>
      Atendimento #{{ atendimento.id }} – {{ atendimento.assunto }}
    </v-card-title>
    <v-card-subtitle class="grey--text text--darken-1">
      Cliente: {{ atendimento.idCliente }} |
      Status: {{ atendimento.status === 1 ? 'Em aberto' : 'Encerrado' }} |
      Iniciado em: {{ formatDate(atendimento.dataCadastro) }} |
      Encerrado em: {{ atendimento.dataEncerramento ? formatDate(atendimento.dataEncerramento) : '-' }}
    </v-card-subtitle>
    <v-divider/>

    <!-- Mensagem de carregamento / erro -->
    <v-card-text>
      <v-skeleton-loader
        v-if="loading"
        type="card"
        height="200px"
      />
      <v-alert v-else-if="error" type="error">{{ error }}</v-alert>
      <div v-else>
        <!-- Histórico geral -->
        <!-- <v-alert
          v-if="atendimento.historico"
          type="info"
          class="mb-4"
          dense
        >
          {{ atendimento.historico }}
        </v-alert> -->

        <!-- Lista de pareceres em cards -->
        <v-subheader>Pareceres</v-subheader>
        <v-row>
          <v-col cols="12" v-for="p in atendimento.pareceres" :key="p.id">
            <v-card class="mb-4" outlined>
              <v-card-title class="d-flex justify-space-between">
                <div>{{ p.idUsuario }} – {{ formatDate(p.dataCadastro) }}</div>
              </v-card-title>
              <v-card-text>
                {{ p.descricao }}
              </v-card-text>
            </v-card>
          </v-col>
          <v-col cols="12" v-if="!atendimento.pareceres.length">
            <em>Nenhum parecer registrado.</em>
          </v-col>
        </v-row>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import atendimentoService from '@/services/atendimento-service'

export default {
  name: 'HistoricoAtendimento',
  props: {
    atendimentoId: {
      type: [String, Number],
      required: true
    }
  },
  setup(props) {
    const loading = ref(false)
    const error = ref('')
    const atendimento = reactive({
      id: null,
      assunto: '',
      idCliente: null,
      dataCadastro: '',
      dataEncerramento: null,
      status: null,
      historico: '',
      pareceres: []
    })

    function formatDate(iso) {
      return new Date(iso).toLocaleString()
    }

    onMounted(async () => {
      loading.value = true
      try {
        const data = await atendimentoService.obterAtendimentoPorId(props.atendimentoId)
        Object.assign(atendimento, {
          id: data.id,
          assunto: data.assunto,
          idCliente: data.idCliente,
          dataCadastro: data.dataCadastro,
          dataEncerramento: data.dataEncerramento,
          status: data.status,
          historico: data.historico,
          pareceres: data.pareceres || []
        })
      } catch (e) {
        console.error(e)
        error.value = 'Não foi possível carregar o histórico do atendimento.'
      } finally {
        loading.value = false
      }
    })

    return { loading, error, atendimento, formatDate }
  }
}
</script>

<style scoped>
/* ajustes finos, se necessário */
</style>
