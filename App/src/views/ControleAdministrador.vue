<template>
  <v-container fluid>
    <v-card class="mb-6" elevation="8" rounded="lg">
      <v-card-title class="titulo"
        >Administrador de Relacionamentos</v-card-title
      >
      <v-divider />
      <v-card-text>
        <v-row align="center" dense>
          <v-col cols="12" sm="2">
            <v-text-field
              :loading="loading"
              v-model="idCliente"
              append-inner-icon="mdi-magnify"
              label="Código"
              placeholder="Código"
              dense
              outlined
              type="text"
              @keyup.enter="buscarCliente"
              @click:append-inner="buscarCliente"
            >
            </v-text-field>
          </v-col>
          <template v-if="cliente">
            <v-col cols="12" sm="6">
              <v-text-field
                v-model="cliente.nome"
                label="Nome"
                dense
                outlined
                readonly
              />
            </v-col>
            <v-col cols="12" sm="2">
              <v-text-field
                :model-value="formatarData(cliente.dataCadastro)"
                label="Data Cadastro"
                dense
                outlined
                readonly
              />
            </v-col>
          </template>
        </v-row>
      </v-card-text>
    </v-card>
    <v-alert v-if="error" type="error" dense class="mb-6">{{ error }}</v-alert>
    <v-row v-if="cliente" dense>
      <v-col cols="12" md="3">
        <v-card class="mb-6" elevation="8">
          <v-card-title>Dados do Cliente</v-card-title>
          <v-divider />
          <v-card-text>
            <v-list dense>
              <v-list-item>
                <v-list-item-icon class="me-2">
                  <v-icon>mdi-fingerprint</v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <strong>CPF/CNPJ:</strong> {{ cliente.cpfCnpj }}
                </v-list-item-content>
              </v-list-item>
              <v-list-item>
                <v-list-item-icon class="me-2">
                  <v-icon>mdi-account</v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <strong>Tipo Pessoa:</strong>
                  {{ formatarTipoPessoa(cliente.tipoPessoa) }}
                </v-list-item-content>
              </v-list-item>
              <v-list-item>
                <v-list-item-icon class="me-2">
                  <v-icon>mdi-email</v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <strong>E-mail:</strong> {{ cliente.email }}
                </v-list-item-content>
              </v-list-item>
              <v-list-item>
                <v-list-item-icon class="me-2">
                  <v-icon>mdi-phone</v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <strong>Telefone:</strong> {{ cliente.telefone }}
                </v-list-item-content>
              </v-list-item>
              <v-list-item>
                <v-list-item-icon class="me-2">
                  <v-icon>mdi-note-text</v-icon>
                </v-list-item-icon>
                <v-list-item-content>
                  <strong>Observação:</strong> {{ cliente.observacao }}
                </v-list-item-content>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="9">
        <v-card
          outlined
          class="d-flex flex-column"
          style="height: 600px;"            
        >
          <div class="pa-2">
            <v-row align="center" justify="space-between" dense>
              <v-radio-group v-model="filtroStatus" row dense>
                <v-radio label="Abertos" value="abertos" />
                <v-radio label="Encerrados" value="encerrados" />
                <v-radio label="Reabertos" value="reabertos" />
              </v-radio-group>
              <v-btn class="mr-3" color="primary" @click="novoAtendimento">
                <v-icon left>mdi-plus</v-icon>
                Novo Atendimento
              </v-btn>
            </v-row>
            <v-divider class="my-2" />
          </div>
          <v-card-text class="flex-grow-1 overflow-y-auto pa-4">
            <v-row dense>
              <v-col
                cols="12"
                sm="6"
                md="6"
                v-for="at in atendimentosFiltrados"
                :key="at.id"
              >
                <v-card class="mb-4" elevation="8" rounded="lg">
                  <v-card-title class="d-flex justify-space-between">
                    <div>
                      <v-icon small class="mr-1">mdi-file-document-outline</v-icon>
                      ATENDIMENTO #{{ at.id }}
                    </div>
                    <v-chip
                      small
                      :color="chipInfo(at.status).color"
                      text-color="white"
                      class="mr-2"
                    >
                      {{ chipInfo(at.status).label }}
                    </v-chip>
                  </v-card-title>
                  <v-card-text>
                    <div class="d-flex align-center mb-2">
                      <v-icon small class="mr-1">mdi-comment-text</v-icon>
                      {{ at.assunto || "Nenhum assunto" }}
                    </div>
                    <div class="d-flex align-center mb-2">
                      <v-icon small class="mr-1">mdi-calendar-clock</v-icon>
                      Iniciado em {{ formataData(at.dataCadastro) }}
                    </div>
                    <div class="d-flex align-center mb-2">
                      <v-icon small class="mr-1">mdi-calendar-check</v-icon>
                      Finalizado em {{ formataData(at.dataEncerramento) ?? "-" }}
                    </div>
                    <div class="d-flex align-center mb-2">
                      <v-icon small class="mr-1">mdi-account-tie</v-icon>
                      Usuario Abertura {{ at.idUsuario }}
                    </div>
                  </v-card-text>
                  <v-card-actions class="justify-end">
                    <v-btn
                      v-if="at.status !== 2"
                      small
                      color="primary"
                      @click="abrirParecer(at)"
                      class="mr-2"
                      rounded="3"
                      variant="flat"
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
          </v-card-text>
          <v-card-actions>
            <v-spacer />
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <NovoAtendimento
      v-model="showForm"
      v-if="showForm"
      :initial="{
        idCliente: idCliente,
        status: 1,
        parecer: { descricao: '', pessoaContato: '' },
      }"
      @save="handleSave"
      @cancel="showForm = false"
    />

    <NovoParecer
      v-model="showParecer"
      v-if="showParecer && selectedAtendimento"
      :atendimento-id="selectedAtendimento.id"
      @saved="handleParecerSaved"
      @cancel="showParecer = false"
    />

    <HistoricoAtendimento
      v-model="showHistorico"
      @click="showHistorico = false"
      v-if="showHistorico && selectedAtendimento"
      :atendimento-id="selectedAtendimento.id"
    />

    <LogsAtendimento
      v-model="showLog"
      v-if="showLog && selectedAtendimento"
      :atendimento-id="selectedAtendimento.id"
      @close="showLog = false"
    />

    <MotivoReabertura
      v-model="showReabrir"
      :atendimento="selectedAtendimento"
      @submitted="handleReabrirSaved"
    />

    <EditarAtendimento
      v-if="selectedAtendimento"
      v-model="showEdicao"
      :atendimento="selectedAtendimento"
      @submitted="handleEditSaved"
    />
  </v-container>
</template>

<script>
import { ref, computed, watch } from "vue";
import { formatarTipoPessoa } from "@/utils/enums";
import { formatarData } from "@/utils/conversor-data";
import { toast } from "vue3-toastify";
import "vue3-toastify/dist/index.css";
import clienteService from "@/services/cliente-service";
import atendimentoService from "@/services/atendimento-service";
import NovoAtendimento from "@/components/atendimento/NovoAtendimento.vue";
import NovoParecer from "@/components/atendimento/NovoParecer.vue";
import HistoricoAtendimento from "@/components/atendimento/HistoricoAtendimento.vue";
import LogsAtendimento from "@/components/atendimento/LogsAtendimento.vue";
import MotivoReabertura from "@/components/atendimento/MotivoReabertura.vue";
import EditarAtendimento from "@/components/atendimento/EditarAtendimento.vue";

export default {
  name: "AtendimentoCliente",
  props: { cliente: Object },
  components: {
    NovoAtendimento,
    NovoParecer,
    HistoricoAtendimento,
    LogsAtendimento,
    MotivoReabertura,
    EditarAtendimento,
  },

  setup() {
    const idCliente = ref(null);
    const cliente = ref(null);
    const loading = ref(false);
    const error = ref("");
    const atendimentos = ref([]);
    const filtroStatus = ref("abertos");
    const showForm = ref(false);
    const showParecer = ref(false);
    const showHistorico = ref(false);
    const showLog = ref(false);
    const showReabrir = ref(false);
    const showEdicao = ref(false);
    const selectedAtendimento = ref(null);
    const atendimentosFiltrados = computed(() => atendimentos.value);
    const statusMap = {
      1: { label: "Em aberto", color: "green" },
      2: { label: "Encerrado", color: "grey" },
      3: { label: "Reaberto", color: "blue" },
    };

    watch(filtroStatus, async (novo) => {
      if (!cliente.value) return;

      const mapStatus = {
        abertos: 1,
        encerrados: 2,
        reabertos: 3,
      };

      const statusNum = mapStatus[novo];
      if (statusNum) {
        await carregarAtendimentos(idCliente.value, statusNum);
      }
    });

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

    function reabrirAtendimento(atendimento) {
      selectedAtendimento.value = atendimento;
      showReabrir.value = true;
    }

    function editarAtendimento(atendimento) {
      selectedAtendimento.value = atendimento;
      showEdicao.value = true;
    }

    function chipInfo(status) {
      return statusMap[status] || { label: "-", color: "grey" };
    }

    function formataData(iso) {
      if (!iso) return "-";
      return new Date(iso).toLocaleString("pt-BR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      });
    }

    async function handleSave(novoData) {
      try {
        await atendimentoService.cadastrarAtendimento(novoData);
        toast.success("Atendimento cadastrado com sucesso!");
        showForm.value = false;
        const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
        await carregarAtendimentos(idCliente.value, statusNum);
      } catch {
        toast.error("Erro ao cadastrar atendimento.");
      }
    }

    async function handleParecerSaved() {
      showParecer.value = false;
      toast.success("Parecer registrado com sucesso!");
      const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
      await carregarAtendimentos(idCliente.value, statusNum);
    }

    async function handleReabrirSaved() {
      showReabrir.value = false;
      toast.success("Atendimento reaberto com sucesso!");
      const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
      await carregarAtendimentos(idCliente.value, statusNum);
    }

    async function handleEditSaved() {
      showEdicao.value = false;
      const statusNum = filtroStatus.value === "abertos" ? 1 : 2;
      await carregarAtendimentos(idCliente.value, statusNum);
    }

    async function handleEditSaved() {
      showEdicao.value = false;
      toast.success("Atendimento editado com sucesso!");
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
      novoAtendimento,
      handleSave,
      showParecer,
      showReabrir,
      showHistorico,
      showLog,
      showEdicao,
      showForm,
      abrirParecer,
      selectedAtendimento,
      handleParecerSaved,
      abrirHistorico,
      abrirLog,
      handleEditSaved,
      editarAtendimento,
      reabrirAtendimento,
      handleReabrirSaved,
      formatarData,
      formatarTipoPessoa,
      formataData,
      chipInfo,
    };
  },
};
</script>

<style scoped>

</style>
