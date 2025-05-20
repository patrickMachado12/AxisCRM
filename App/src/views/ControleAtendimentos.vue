<template>
  <v-container fluid>
    <!-- CARD de Filtros -->
    <v-card class="mb-6">
      <v-card-title>Filtrar Atendimentos</v-card-title>
      <v-divider />
      <v-card-text>
        <v-row align="center" dense>
          <!-- Usuário -->
          <v-col cols="12" sm="3">
            <v-select
              v-model="filters.userId"
              :items="users"
              item-title="email"
              item-value="id"
              :return-object="false"
              label="Usuário"
              dense
              outlined
              clearable
            />
          </v-col>

          <!-- Cliente -->
          <v-col cols="12" sm="3">
            <v-select
              v-model="filters.clientId"
              :items="clients"
              item-title="nome"
              item-value="id"
              :return-object="false"
              label="Cliente"
              dense
              outlined
              clearable
            />
          </v-col>

          <!-- Status -->
          <v-col cols="12" sm="2">
            <v-select
              v-model="filters.status"
              :items="statusOptions"
              :return-object="false"
              label="Status"
              dense
              outlined
              clearable
            />
          </v-col>

          <!-- Data Inicial -->
          <v-col cols="12" sm="2">
            <v-menu
              v-model="menuStart"
              :close-on-content-click="false"
              transition="scale-transition"
              offset-y
              min-width="auto"
            >
              <template #activator="{ props }">
                <v-text-field
                  v-model="filters.startDate"
                  label="Data Inicial"
                  readonly
                  dense
                  outlined
                  clearable
                  v-bind="props"
                />
              </template>
              <v-date-picker
                v-model="filters.startDate"
                @update:model-value="menuStart = false"
              />
            </v-menu>
          </v-col>

          <!-- Data Final -->
          <v-col cols="12" sm="2">
            <v-menu
              v-model="menuEnd"
              :close-on-content-click="false"
              transition="scale-transition"
              offset-y
              min-width="auto"
            >
              <template #activator="{ props }">
                <v-text-field
                  v-model="filters.endDate"
                  label="Data Final"
                  readonly
                  dense
                  outlined
                  clearable
                  v-bind="props"
                />
              </template>
              <v-date-picker
                v-model="filters.endDate"
                @update:model-value="menuEnd = false"
              />
            </v-menu>
          </v-col>

          <!-- Botão Filtrar -->
          <v-col cols="12" class="text-end">
            <v-btn color="primary" @click="applyFilters"> Filtrar </v-btn>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <!-- GRID de resultados só depois de filtrar -->
    <v-row dense v-if="filterExecuted">
      <!-- Coluna de Clientes -->
      <v-col cols="12" md="3">
        <v-card class="mb-6" outlined>
          <v-card-title>Clientes ({{ uniqueClients.length }})</v-card-title>
          <v-divider />
          <v-card-text>
            <v-list dense shaped>
              <v-list-item
                v-for="cl in uniqueClients"
                :key="cl.id"
                :active="cl.id === selectedClientId"
                @click="selectClient(cl.id)"
                class="clickable"
              >
                <v-list-item-content>
                  <v-list-item-title>{{ cl.nome }}</v-list-item-title>
                </v-list-item-content>
              </v-list-item>
              <v-list-item v-if="!uniqueClients.length">
                <v-list-item-content>Nenhum cliente</v-list-item-content>
              </v-list-item>
            </v-list>
          </v-card-text>
          <v-card-actions>
            <v-btn text small @click="selectClient(null)">
              Mostrar todos
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- Coluna de Atendimentos -->
      <v-col cols="12" md="9">
        <v-card class="pa-4" outlined>
          <v-row dense>
            <v-col
              cols="12"
              sm="6"
              md="4"
              v-for="att in displayedAtendimentos"
              :key="att.id"
            >
              <v-card class="mb-4" elevation="8" rounded="lg">
                <v-card-title class="d-flex justify-space-between">
                  <div>#{{ att.id }} – {{ att.assunto }}</div>
                  <v-chip
                    small
                    :color="chipColor(att.status)"
                    text-color="white"
                  >
                    {{ att.status }}
                  </v-chip>
                </v-card-title>
                <v-card-text>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-calendar-clock</v-icon>
                    {{ formatDate(att.dataCadastro) }}
                  </div>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-comment-text</v-icon>
                    {{ att.historico || "Nenhum histórico" }}
                  </div>
                </v-card-text>
                <v-card-actions class="justify-end">
                  <v-btn
                    v-if="att.status !== 2"
                    small
                    color="secondary"
                    @click="abrirParecer(att)"
                    title="Parecer"
                  >
                    <v-icon left>mdi mdi-chat-plus</v-icon>
                  </v-btn>
                  <v-menu offset-y attach="body" location="top">
                    <template #activator="{ props }">
                      <v-btn icon v-bind="props">
                        <v-icon>mdi-dots-vertical</v-icon>
                      </v-btn>
                    </template>

                    <v-list dense>
                      <v-list-item @click="abrirHistorico(att)">
                        <v-list-item-title
                          >Histórico do atendimento</v-list-item-title
                        >
                      </v-list-item>

                      <v-list-item @click="abrirLog(att)">
                        <v-list-item-title>Log de alteração</v-list-item-title>
                      </v-list-item>

                      <v-list-item
                        v-if="att.status === 1 || att.status === 3"
                        @click="editarAtendimento(att)"
                      >
                        <v-list-item-title
                          >Editar atendimento</v-list-item-title
                        >
                      </v-list-item>

                      <v-list-item
                        v-if="att.status === 2"
                        @click="reabrirAtendimento(att)"
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
            <v-col cols="12" v-if="!displayedAtendimentos.length">
              <p>Nenhum atendimento para exibir.</p>
            </v-col>
          </v-row>
        </v-card>
      </v-col>
    </v-row>

    <!-- DIALOG: Novo Atendimento -->
    <v-dialog v-model="showForm" max-width="700" persistent>
      <v-card>
        <v-card-title>Novo atendimento</v-card-title>
        <v-divider />
        <v-card-text>
          <!-- só montamos o form quando o diálogo estiver visível -->
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

    <!-- DIALOG: Parecer de Atendimento -->
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

    <!-- DIALOG: Histórico de Atendimento -->
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

    <!-- DIALOG: Logs de Alterações -->
    <v-dialog v-model="showLog" max-width="600" persistent>
      <LogsAtendimento
        v-if="showLog && selectedAtendimento"
        :atendimento-id="selectedAtendimento.id"
        @close="showLog = false"
      />
    </v-dialog>

    <!-- diálogo de Motivo de Reabertura -->
    <MotivoReabertura
      v-model="showReabrir"
      :atendimento="selectedAtendimento"
      @submitted="handleReabrirSaved"
    />
  </v-container>
</template>

<script>
import { ref, reactive, computed, onMounted } from "vue";
import api from "@/services/api";
import clienteService from "@/services/cliente-service";
import usuarioService from "@/services/usuario-service";
import CampoData from "@/components/date/CampoData.vue";
import NovoAtendimento from "@/components/NovoAtendimento.vue";
import NovoParecer from "@/components/NovoParecer.vue";
import HistoricoAtendimento from "@/components/HistoricoAtendimento.vue";
import LogsAtendimento from "@/components/LogsAtendimento.vue";
import MotivoReabertura from "@/components/MotivoReabertura.vue";

export default {
  name: "FiltroAtendimentos",
  components: {
    CampoData,
    NovoAtendimento,
    NovoParecer,
    HistoricoAtendimento,
    LogsAtendimento,
    MotivoReabertura,
  },

  setup() {
    const menuStart = ref(false);
    const menuEnd = ref(false);
    const filterExecuted = ref(false);

    // Diálogo de Novo Atendimento
    const showForm = ref(false);

    // Diálogo de Parecer
    const showParecer = ref(false);
    const selectedAtendimento = ref(null);

    // Diálogo de Histórico
    const showHistorico = ref(false);

    // Diálogo de Log
    const showLog = ref(false);

    // Diálogo de Reabertura
    const showReabrir = ref(false);

    const filters = reactive({
      userId: null,
      clientId: null,
      status: null,
      startDate: null,
      endDate: null,
    });

    const users = ref([]);
    const clients = ref([]);
    const atendimentos = ref([]);
    const selectedClientId = ref(null);

    const statusOptions = ["Aberto", "Encerrado", "Reaberto"];

    onMounted(async () => {
      try {
        const rU = await usuarioService.obterTodos();
        const u = rU.data?.itens ?? rU.data ?? [];
        users.value = Array.isArray(u) ? u : [];
      } catch {
        users.value = [];
      }

      try {
        const rC = await clienteService.obterTodos();
        const c = rC.data?.itens ?? rC.data ?? [];
        clients.value = Array.isArray(c) ? c : [];
      } catch {
        clients.value = [];
      }
    });

    const uniqueClients = computed(() => {
      const map = new Map();
      atendimentos.value.forEach((a) => {
        if (!map.has(a.idCliente)) {
          const cli = clients.value.find((x) => x.id === a.idCliente);
          map.set(a.idCliente, cli || { id: a.idCliente, nome: "—" });
        }
      });
      return Array.from(map.values());
    });

    const displayedAtendimentos = computed(() => {
      if (selectedClientId.value !== null) {
        return atendimentos.value.filter(
          (a) => a.idCliente === selectedClientId.value
        );
      }
      return atendimentos.value;
    });

    async function applyFilters() {
      const params = {};
      if (filters.userId) params.idUsuario = filters.userId;
      if (filters.clientId) params.idCliente = filters.clientId;
      if (filters.status) params.status = filters.status;
      if (filters.startDate) params.dataInicial = `${filters.startDate} 00:00`;
      if (filters.endDate) params.dataFinal = `${filters.endDate} 23:59`;

      try {
        const r = await api.get("/atendimentos", { params });
        const b = r.data ?? r;
        atendimentos.value = Array.isArray(b)
          ? b
          : Array.isArray(b.itens)
          ? b.itens
          : [];
      } catch {
        atendimentos.value = [];
      } finally {
        filterExecuted.value = true;
      }
    }

    function selectClient(id) {
      selectedClientId.value = id;
    }

    function formatDate(iso) {
      if (!iso) return "-";
      return new Date(iso).toLocaleString("pt-BR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      });
    }

    function chipColor(status) {
      return status === "Encerrado"
        ? "grey"
        : status === "Aberto"
        ? "green"
        : "blue";
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

    async function handleSave(novoData) {
      try {
        await atendimentoService.cadastrarAtendimento(novoData);
        showForm.value = false;
        await applyFilters();
      } catch (e) {
        console.error(e);
        error.value = "Erro ao salvar atendimento.";
      }
    }

    async function handleParecerSaved() {
      showParecer.value = false;
      await applyFilters();
    }

    async function handleReabrirSaved() {
      showReabrir.value = false;
      await applyFilters();
    }

    const viewLog = (_) => {};
    const respond = (_) => {};
    const remove = (_) => {};
    const finalize = (_) => {};

    return {
      menuStart,
      menuEnd,
      filters,
      filterExecuted,
      users,
      clients,
      statusOptions,
      atendimentos,
      uniqueClients,
      displayedAtendimentos,
      selectedClientId,
      applyFilters,
      selectClient,
      formatDate,
      chipColor,
      viewLog,
      respond,
      remove,
      finalize,

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

      // Reabertura
      showReabrir,
      reabrirAtendimento,
      handleReabrirSaved,
    };
  },
};
</script>

<style scoped></style>
