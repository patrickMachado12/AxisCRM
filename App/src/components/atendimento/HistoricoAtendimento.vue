<template>
  <v-dialog v-model="showHistorico" max-width="800" persistent>
    <v-card class="historico-card d-flex flex-column" style="height: 100%">
      <div>
        <v-card-title>
          Atendimento #{{ atendimento.id }} – {{ atendimento.assunto }}
        </v-card-title>
        <v-card-subtitle class="grey--text text--darken-1">
          Cliente: {{ atendimento.idCliente }} | Status:
          {{ atendimento.status === 1 ? "Em aberto" : "Encerrado" }} | Iniciado
          em: {{ formatDate(atendimento.dataCadastro) }} | Encerrado em:
          {{
            atendimento.dataEncerramento
              ? formatDate(atendimento.dataEncerramento)
              : "-"
          }}
        </v-card-subtitle>
        <v-divider />
      </div>
      <v-card-text class="flex-grow-1 overflow-y-auto pa-4">
        <v-skeleton-loader v-if="loading" type="card" height="200px" />
        <v-alert v-else-if="error" type="error">{{ error }}</v-alert>
        <div v-else>
          <v-row>
            <v-col cols="12" v-for="p in atendimento.pareceres" :key="p.id">
              <v-card class="mb-4" outlined elevation="3">
                <v-card-subtitle class="d-flex justify-space-between">
                  <div>
                    <div class="d-flex align-center mb-1">
                      <strong>Data Cadastro:</strong>
                      {{ formatDate(p.dataCadastro) }}
                    </div>
                    <div class="d-flex align-center mb-1">
                      <strong>Última Alteração:</strong>
                      {{ formatDate(p.dataUltimaAlteracao) }}
                    </div>
                    <div class="d-flex align-center mb-1">
                      <strong>Usuário:</strong> {{ p.idUsuario }}
                    </div>
                    <div class="d-flex align-center">
                      <strong>Contato:</strong>
                      {{ p.pessoaContato ?? "-" }}
                    </div>
                  </div>
                  <v-icon
                    v-if="atendimento.status === 1 || atendimento.status === 3"
                    small
                    class="mi-1"
                    @click.stop="editarParecer(p)"
                    title="Editar parecer"
                  >
                    mdi-pencil
                  </v-icon>
                </v-card-subtitle>
                <v-card-text class="bg-surface-light pt-4">
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
      <v-divider />
      <v-card-actions>
        <v-btn class="mr-2" variant="tonal" @click="showHistorico = false">
          Fechar
        </v-btn>
      </v-card-actions>

      <EditarParecer
        v-if="selectedParecer"
        v-model="showEditParecer"
        :parecer="selectedParecer"
        :atendimento-id="atendimento.id"
        @submitted="onParecerUpdated"
      />
    </v-card>
  </v-dialog>
</template>

<script>
import { ref, reactive, onMounted } from "vue";
import atendimentoService from "@/services/atendimento-service";
import EditarParecer from "@/components/atendimento/EditarParecer.vue";

export default {
  name: "HistoricoAtendimento",
  components: { EditarParecer },
  props: {
    atendimentoId: {
      type: [String, Number],
      required: true,
    },
  },
  emits: ["editParecer"],

  setup(props, { emit }) {
    const loading = ref(false);
    const error = ref("");
    const atendimento = reactive({
      id: null,
      assunto: "",
      idCliente: null,
      dataCadastro: "",
      dataEncerramento: null,
      status: null,
      historico: "",
      pareceres: [],
    });

    const showEditParecer = ref(false);
    const selectedParecer = ref(null);

    onMounted(loadHistorico);

    function formatDate(iso) {
      return new Date(iso).toLocaleString();
    }

    async function loadHistorico() {
      loading.value = true;
      error.value = "";
      try {
        const data = await atendimentoService.obterAtendimentoPorId(
          props.atendimentoId
        );
        Object.assign(atendimento, {
          id: data.id,
          assunto: data.assunto,
          idCliente: data.idCliente,
          dataCadastro: data.dataCadastro,
          dataEncerramento: data.dataEncerramento,
          status: data.status,
          historico: data.historico,
          pareceres: data.pareceres || [],
        });
      } catch (e) {
        console.error(e);
        error.value = "Não foi possível carregar o histórico do atendimento.";
      } finally {
        loading.value = false;
      }
    }

    function editarParecer(parecer) {
      selectedParecer.value = parecer;
      showEditParecer.value = true;
    }

    async function onParecerUpdated() {
      showEditParecer.value = false;
      selectedParecer.value = null;
      await loadHistorico();
    }

    return {
      loading,
      error,
      atendimento,
      formatDate,
      showEditParecer,
      selectedParecer,
      editarParecer,
      onParecerUpdated,
    };
  },
};
</script>

<style scoped></style>
