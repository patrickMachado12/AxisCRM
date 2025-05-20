<template>
  <v-container fluid>
    <!-- Busca de cliente -->
    <v-card class="mb-6">
      <v-card-title>Administrador de Relacionamentos</v-card-title>
      <v-divider />
      <v-card-text>
        <v-row align="center" dense>
          <v-col cols="12" sm="2">
            <v-text-field
              v-model="idCliente"
              label="Código"
              placeholder="Código"
              dense
              outlined
              clearable
              type="number"
              @keyup.enter="buscarCliente"
            >
              <template #append>
                <v-icon @click="buscarCliente" :loading="loading"
                  >mdi-magnify</v-icon
                >
              </template>
            </v-text-field>
          </v-col>

          <template v-if="cliente">
            <v-col cols="12" sm="4">
              <v-text-field
                v-model="cliente.nome"
                label="Nome"
                dense
                outlined
                readonly
              />
            </v-col>
            <v-col cols="12" sm="4">
              <v-text-field
                v-model="cliente.email"
                label="E-mail"
                dense
                outlined
                readonly
              />
            </v-col>
          </template>
        </v-row>
      </v-card-text>
    </v-card>

    <!-- Mensagem de erro -->
    <v-alert v-if="error" type="error" dense class="mb-6">{{ error }}</v-alert>

    <!-- Resumo e lista de atendimentos -->
    <v-row v-if="cliente" dense>
      <!-- Resumo do cliente -->
      <v-col cols="12" md="3">
        <v-card class="mb-6">
          <v-card-title>Dados do Cliente ({{ cliente.id }})</v-card-title>
          <v-divider />
          <v-card-text>
            <v-list dense>
              <v-list-item>
                <v-list-item-content
                  ><strong>CPF/CNPJ:</strong>
                  {{ cliente.cpfCnpj }}</v-list-item-content
                >
              </v-list-item>
              <v-list-item>
                <v-list-item-content
                  ><strong>E-mail:</strong>
                  {{ cliente.email }}</v-list-item-content
                >
              </v-list-item>
              <v-list-item>
                <v-list-item-content
                  ><strong>Tipo:</strong>
                  {{ cliente.tipoPessoa }}</v-list-item-content
                >
              </v-list-item>
              <v-list-item>
                <v-list-item-content
                  ><strong>Telefone:</strong>
                  {{ cliente.telefone }}</v-list-item-content
                >
              </v-list-item>
              <v-list-item>
                <v-list-item-content
                  ><strong>Observação:</strong>
                  {{ cliente.observacao }}</v-list-item-content
                >
              </v-list-item>
              <v-list-item>
                <v-list-item-content
                  ><strong>Data Cadastro:</strong>
                  {{ cliente.dataCadastro }}</v-list-item-content
                >
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="9">
        <v-card class="pa-4">
          <v-row align="center" justify="space-between">
            <v-radio-group v-model="filtroStatus" row dense>
              <v-radio label="Abertos" value="abertos" />
              <v-radio label="Encerrados" value="encerrados" />
            </v-radio-group>
            <v-btn color="primary" @click="novoAtendimento">
              <v-icon left>mdi-plus</v-icon>
              Novo Atendimento
            </v-btn>
          </v-row>

          <v-divider class="my-4" />

          <!-- Lista dinâmica de atendimentos -->
          <v-row dense>
            <v-col cols="12" v-for="at in atendimentosFiltrados" :key="at.id">
              <v-card class="mb-4" elevation="2" rounded="lg">
                <v-card-title class="d-flex justify-space-between">
                  <div>ATENDIMENTO #000{{ at.id }} – {{ at.assunto }}</div>
                  <div class="d-flex align-center">
                    <v-chip
                      small
                      :color="at.status === 2 ? 'grey' : 'green'"
                      text-color="white"
                      class="mr-2"
                    >
                      {{ at.status === 2 ? "Encerrado" : "Em aberto" }}
                    </v-chip>
                  </div>
                </v-card-title>

                <v-card-text>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-calendar-clock</v-icon>
                    Iniciado em {{ at.dataCadastro }}
                  </div>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-calendar-check</v-icon>
                    Finalizado em {{ at.dataEncerramento ?? "-" }}
                  </div>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-comment-text</v-icon>
                    {{ at.historico || "Nenhuma observação" }}
                  </div>
                </v-card-text>

                <v-card-actions class="justify-end">
                  <v-btn
                    v-if="at.status !== 2"
                    small
                    color="primary"
                    @click="abrirParecer(at)"
                  >
                    Parecer
                  </v-btn>

                  <v-menu offset-y attach="body">
                    <template #activator="{ props }">
                      <v-btn icon v-bind="props">
                        <v-icon>mdi-dots-vertical</v-icon>
                      </v-btn>
                    </template>

                    <v-list dense>
                      <v-list-item @click="abrirHistorico(at)">
                        <v-list-item-title
                          >Histórico do atendimento</v-list-item-title
                        >
                      </v-list-item>

                      <v-list-item @click="abrirLog(at)">
                        <v-list-item-title>Log de alteração</v-list-item-title>
                      </v-list-item>

                      <v-list-item
                        v-if="at.status === 1 || at.status === 3"
                        @click="editarAtendimento(at)"
                      >
                        <v-list-item-title
                          >Editar atendimento</v-list-item-title
                        >
                      </v-list-item>

                      <v-list-item
                        v-if="at.status === 2"
                        @click="reabrirAtendimento(at)"
                      >
                        <v-list-item-title
                          >Reabrir atendimento</v-list-item-title
                        >
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </v-card-actions>
              </v-card>
            </v-col>
            <v-col cols="12" v-if="!atendimentosFiltrados.length">
              <p>Nenhum atendimento para exibir.</p>
            </v-col>
          </v-row>
        </v-card>
      </v-col>
    </v-row>

    <v-dialog v-model="showForm" max-width="700" persistent>
      <v-card>
        <v-card-title>Novo atendimento</v-card-title>
        <v-divider />
        <v-card-text>
          <NovoAtendimento
            v-if="showForm"
            :initial="{
              idCliente: idCliente,
              status: 1,
              parecer: { descricao: '', pessoaContato: '' },
            }"
            @save="handleSave"
            @cancel="showForm = false"
          />
        </v-card-text>
      </v-card>
    </v-dialog>

    <v-dialog v-model="showParecer" max-width="600" persistent>
      <v-card>
        <v-card-title>Parecer</v-card-title>
        <v-divider />
        <v-card-text>
          <NovoParecer
            v-if="showParecer && selectedAtendimento"
            :atendimento-id="selectedAtendimento.id"
            @saved="handleParecerSaved"
            @cancel="showParecer = false"
          />
        </v-card-text>
      </v-card>
    </v-dialog>

    <v-dialog v-model="showHistorico" max-width="800" persistent>
      <v-card>
        <v-card-title>Histórico Atendimento</v-card-title>
        <v-divider />
        <v-card-text>
          <HistoricoAtendimento
            v-if="showHistorico && selectedAtendimento"
            :atendimento-id="selectedAtendimento.id"
          />
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn text @click="showHistorico = false">Fechar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="showLog" max-width="600" persistent>
      <LogsAtendimento
        v-if="showLog && selectedAtendimento"
        :atendimento-id="selectedAtendimento.id"
        @close="showLog = false"
      />
    </v-dialog>
    
    <EditarAtendimento
      v-if="selectedAtendimento"
      v-model="showEdit"
      :atendimento="selectedAtendimento"
      @submitted="handleEditSaved"
    />
  </v-container>
</template>

<script>
import { ref, computed, watch } from "vue";
import clienteService from "@/services/cliente-service";
import atendimentoService from "@/services/atendimento-service";
import NovoAtendimento from "@/components/NovoAtendimento.vue";
import NovoParecer from "@/components/NovoParecer.vue";
import HistoricoAtendimento from "@/components/HistoricoAtendimento.vue";
import LogsAtendimento from "@/components/LogsAtendimento.vue";
import EditarAtendimento from "@/components/EditarAtendimento.vue";

export default {
  name: "AtendimentoCliente",
  components: {
    NovoAtendimento,
    NovoParecer,
    HistoricoAtendimento,
    LogsAtendimento,
    EditarAtendimento,
  },

  setup() {
    const idCliente = ref(null);
    const cliente = ref(null);
    const loading = ref(false);
    const error = ref("");
    const atendimentos = ref([]);
    const filtroStatus = ref("abertos");

    // Diálogo de Novo Atendimento
    const showForm = ref(false);

    // Diálogo de Parecer
    const showParecer = ref(false);
    const selectedAtendimento = ref(null);

    // Diálogo de Histórico
    const showHistorico = ref(false);

    // Diálogo de Log
    const showLog = ref(false);

    // Diálogo de Editação de Atendimento
    const showEdit = ref(false);

    const atendimentosFiltrados = computed(() => atendimentos.value);

    async function carregarAtendimentos(idCli, status) {
      loading.value = true;
      try {
        atendimentos.value = await atendimentoService.obterAtendimentoFiltrado(
          undefined,
          idCli,
          status
        );
      } catch {
        error.value = "Erro ao carregar atendimentos.";
      } finally {
        loading.value = false;
      }
    }

    async function buscarCliente() {
      if (!idCliente.value) {
        error.value = "Informe um código de cliente válido.";
        return;
      }
      loading.value = true;
      error.value = "";
      try {
        cliente.value = await clienteService.obterPorId(idCliente.value);
        await carregarAtendimentos(idCliente.value, 1);
      } catch {
        error.value = "Erro ao buscar cliente ou atendimentos.";
      } finally {
        loading.value = false;
      }
    }

    watch(filtroStatus, async (novo) => {
      if (!cliente.value) return;
      const statusNum = novo === "abertos" ? 1 : 2;
      await carregarAtendimentos(idCliente.value, statusNum);
    });

    function novoAtendimento() {
      showForm.value = true;
    }

    function abrirParecer(atendimento) {
      selectedAtendimento.value = atendimento;
      showParecer.value = true;
    }

    function abrirHistorico(atendimento) {
      selectedAtendimento.value = atendimento;
      showHistorico.value = true;
    }

    function abrirLog(atendimento) {
      selectedAtendimento.value = atendimento;
      showLog.value = true;
    }

    function editarAtendimento(atendimento) {
      selectedAtendimento.value = atendimento;
      showEdit.value = true;
    }

    async function reabrirAtendimento(at) {
      try {
        await atendimentoService.alterarStatusAtendimento(at.id);
        const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
        await carregarAtendimentos(idCliente.value, statusNum);
      } catch (e) {
        console.error("Erro ao reabrir atendimento:", e);
      }
    }

    async function handleSave(novoData) {
      try {
        await atendimentoService.cadastrarAtendimento(novoData);
        showForm.value = false;
        const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
        await carregarAtendimentos(idCliente.value, statusNum);
      } catch (e) {
        console.error(e);
        error.value = "Erro ao salvar atendimento.";
      }
    }

    async function handleParecerSaved() {
      showParecer.value = false;
      const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
      await carregarAtendimentos(idCliente.value, statusNum);
    }

    async function handleEditSaved() {
      showEdit.value = false;
      const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
      await carregarAtendimentos(idCliente.value, statusNum);
    }

    return {
      idCliente,
      cliente,
      loading,
      error,
      atendimentos,
      filtroStatus,
      buscarCliente,
      atendimentosFiltrados,

      // Novo Atendimento
      showForm,
      novoAtendimento,
      handleSave,

      // Parecer
      showParecer,
      abrirParecer,
      selectedAtendimento,
      handleParecerSaved,

      // Histórico
      showHistorico,
      abrirHistorico,

      // Log
      showLog,
      abrirLog,

      // Editação
      showEdit,
      handleEditSaved,
      editarAtendimento,

      // Reabertura
      reabrirAtendimento,
    };
  },
};
</script>

<style scoped></style>
