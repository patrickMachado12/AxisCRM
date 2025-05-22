<template>
  <v-container fluid>
    <v-card class="mb-6" elevation="8" rounded="lg">
      <v-card-title>CRM Atendimentos</v-card-title>
      <v-divider />
      <v-card-text>
        <v-row align="center" dense>
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
                  type="date"
                  dense
                  outlined
                  v-bind="props"
                />
              </template>
            </v-menu>
          </v-col>
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
                  type="date"
                  dense
                  outlined
                  v-bind="props"
                />
              </template>
            </v-menu>
          </v-col>
          <v-col cols="12" class="text-end">
            <v-btn color="primary" @click="aplicaFiltro"> Filtrar </v-btn>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
    <v-row dense v-if="filterExecuted">
      <v-col cols="12" md="3" elevetion="8">
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
        </v-card>
      </v-col>
      <v-col cols="12" md="9">
        <v-card class="pa-4" outlined>
          <v-row dense>
            <v-col
              cols="12"
              sm="6"
              md="6"
              v-for="att in displayedAtendimentos"
              :key="att.id"
            >
              <v-card class="mb-4" elevation="8" rounded="lg">
                <v-card-title class="d-flex justify-space-between">
                  <v-card-title>ATENDIMENTO #{{ att.id }}</v-card-title>
                  <v-chip
                    small
                    :color="chipInfo(att.status).color"
                    text-color="white"
                    class="mr-2"
                  >
                    {{ chipInfo(att.status).label }}
                  </v-chip>
                </v-card-title>
                <v-card-text>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-comment-text</v-icon>
                    {{ att.assunto || "Nenhum assunto" }}
                  </div>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-calendar-clock</v-icon>
                    Iniciado em {{ formataData(att.dataCadastro) }}
                  </div>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-calendar-check</v-icon>
                    Finalizado em {{ formataData(att.dataEncerramento) ?? "-" }}
                  </div>
                  <div class="d-flex align-center mb-2">
                    <v-icon small class="mr-1">mdi-account-tie</v-icon>
                    Usuário Abertura {{ att.idUsuario }}
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
    <MotivoReabertura
      v-model="showReabrir"
      :atendimento="selectedAtendimento"
      @submitted="handleReabrirSaved"
    />
    <EditarAtendimento
      v-if="selectedAtendimento"
      v-model="showEdit"
      :atendimento="selectedAtendimento"
      @submitted="handleEditSaved"
    />
  </v-container>
</template>

<script>
import { ref, reactive, computed, onMounted } from "vue";
import { toast } from "vue3-toastify";
import "vue3-toastify/dist/index.css";
import api from "@/services/api";
import clienteService from "@/services/cliente-service";
import usuarioService from "@/services/usuario-service";
import NovoAtendimento from "@/components/atendimento/NovoAtendimento.vue";
import NovoParecer from "@/components/atendimento/NovoParecer.vue";
import HistoricoAtendimento from "@/components/atendimento/HistoricoAtendimento.vue";
import LogsAtendimento from "@/components/atendimento/LogsAtendimento.vue";
import MotivoReabertura from "@/components/atendimento/MotivoReabertura.vue";
import EditarAtendimento from "@/components/atendimento/EditarAtendimento.vue";

export default {
  name: "FiltroAtendimentos",
  components: {
    NovoAtendimento,
    NovoParecer,
    HistoricoAtendimento,
    LogsAtendimento,
    MotivoReabertura,
    EditarAtendimento,
  },

  setup() {
    const menuStart = ref(false);
    const menuEnd = ref(false);
    const filterExecuted = ref(false);
    const showForm = ref(false);
    const showParecer = ref(false);
    const selectedAtendimento = ref(null);
    const showHistorico = ref(false);
    const showLog = ref(false);
    const showReabrir = ref(false);
    const showEdit = ref(false);

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

    const statusMap = {
      1: { label: "Em aberto",   color: "green" },
      2: { label: "Encerrado",    color: "grey"  },
      3: { label: "Reaberto",     color: "blue"  },
    }

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

    async function aplicaFiltro() {
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

    function chipInfo(status) {
      return statusMap[status] || { label: "-", color: "grey" }
    }


    function selectClient(id) {
      selectedClientId.value = id;
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

    function editarAtendimento(atendimento) {
      selectedAtendimento.value = atendimento;
      showEdit.value = true;
    }

    async function handleSave(novoData) {
      try {
        await atendimentoService.cadastrarAtendimento(novoData);
        toast.success("Atendimento cadastrado com sucesso!");
        showForm.value = false;
        await aplicaFiltro();
      } catch (e) {
        toast.error("Erro ao cadastrar atendimento.");
        console.error(e);
      }
    }

    async function handleParecerSaved() {
      showParecer.value = false;
      toast.success("Parecer registrado com sucesso!");
      await aplicaFiltro();
    }

    async function handleReabrirSaved() {
      showReabrir.value = false;
      toast.success("Atendimento reaberto com sucesso!");
      await aplicaFiltro();
    }

    async function handleEditSaved() {
      showEdit.value = false;
      toast.success("Atendimento editado com sucesso!");
      await aplicaFiltro();
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
      aplicaFiltro,
      selectClient,
      formataData,
      chipColor,
      viewLog,
      respond,
      remove,
      finalize,
      showForm,
      novoAtendimento,
      handleSave,
      showParecer,
      abrirParecer,
      selectedAtendimento,
      handleParecerSaved,
      showHistorico,
      abrirHistorico,
      showLog,
      abrirLog,
      showEdit,
      handleEditSaved,
      editarAtendimento,
      aplicaFiltro,
      showReabrir,
      reabrirAtendimento,
      handleReabrirSaved,
      chipInfo,
    };
  },
};
</script>

<style scoped></style>
