<template>
  <v-container fluid>
    <v-row class="d-flex align-center mb-4">
      <v-col cols="12">
        <v-card-title>Cadastro de Clientes</v-card-title>
        <v-divider />
      </v-col>
      <v-col cols="12">
        <v-btn
          id="btn-cadastrar"
          color="primary"
          @click="abrirDialog()"
        >
          Cadastrar
        </v-btn>
      </v-col>
    </v-row>
    <v-dialog
      v-model="dialog"
      persistent
      max-width="600px"
      :fullscreen="smAndDown"
    >
      <v-card>
        <v-card-title>
          <span class="text-h5">{{ formTitulo }}</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12" sm="5">
                <v-radio-group 
                  inline 
                  label="Tipo Pessoa" 
                  v-model="editedItem.tipoPessoa"
                  :items="tiposPessoaOptions"
                  required
                >
                  <v-radio label="Física" :value="1"></v-radio>
                  <v-radio label="Jurídica" :value="2"></v-radio>
                </v-radio-group>
              </v-col>
              <v-col cols="12" sm="12">
                <v-text-field
                  v-model="editedItem.nome"
                  label="Nome"
                  required
                />
              </v-col>
              <v-col cols="12" sm="12">
                <v-text-field
                  v-model="editedItem.cpfCnpj"
                  label="CPF / CNPJ"
                  required
                />
              </v-col>
              <v-col cols="12" sm="12">
                <v-text-field
                  type="tel"
                  v-model="editedItem.telefone"
                  label="Telefone"
                  required
                />
              </v-col>
              <v-col cols="12" sm="12">
                <v-text-field
                  v-model="editedItem.email"
                  label="E-mail"
                  required
                />
              </v-col>
              <v-col cols="12" sm="12">
                <v-text-field
                  v-model="editedItem.observacao"
                  label="Observação"
                />
              </v-col>          
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
           id="btn-gravar" 
           color="primary" 
           @click="gravar"
          >
            Gravar
          </v-btn>
          <v-btn text @click="dialog = false">Fechar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-row>
      <v-col cols="12">
        <v-data-table-server
          :headers="headers"
          :items="clientes"
          :loading="loading"
          v-model:page="pagina"
          v-model:items-per-page="itensPorPagina"
          :items-length="totalItens"
          class="elevation-8"
        >
          <template #item.tipoPessoa="{ item }">
            {{ tiposPessoaOptions.find(p => p.value === item.tipoPessoa)?.label }}
          </template>
          <template #item.excluido="{ item }">
            <v-chip
              small
              :color="!item.excluido ? 'green' : 'red'"
              text-color="white"
              class="mr-2"
            >
              {{ !item.excluido ? 'Ativo' : 'Excluído' }}
            </v-chip>
          </template>
          <template #item.dataCadastro="{ item }">
            {{ formatDate(item.dataCadastro) }}
          </template>
          <template #item.dataExclusao="{ item }">
            {{ formatDate(item.dataExclusao) }}
          </template>
          <template #item.actions="{ item }">
            <v-icon small class="mr-2" @click="editItem(item)">
              mdi-pencil
            </v-icon>
            <v-icon small @click="promptDelete(item)">
              mdi-delete
            </v-icon>
          </template>
        </v-data-table-server>
      </v-col>
    </v-row>

    <v-dialog v-model="confirmDialog" max-width="400px">
      <v-card>
        <v-card-title class="text-h6">
          Confirmar exclusão
        </v-card-title>
        <v-card-text>
          Tem certeza que deseja excluir
          <strong>{{ itemParaExcluir?.nome }}</strong>?
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn id="btn-confirm-delete" @click="deleteItemConfirmado">
            Sim, excluir
          </v-btn>
          <v-btn text @click="confirmDialog = false">
            Não
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { useDisplay } from 'vuetify'
import { useClientes } from '@/composables/useClientes'
import { toast } from 'vue3-toastify'
import 'vue3-toastify/dist/index.css'

const { 
  clientes, 
  loading, 
  totalItens, 
  loadClientes, 
  saveCliente, 
  deleteCliente 
} = useClientes()
const dialog = ref(false)
const editedItem = ref({})
const message = ref('')
const { smAndDown } = useDisplay()

const tiposPessoaOptions = [
  { label: 'Física', value: 1 },
  { label: 'Jurídica', value: 2 }
]

const statusOptions = [
  { label: 'Ativo', value: false },
  { label: 'Excluído', value: true }
]

const headers = [
  { title: "Código", value: "id", align: "start", sortable: true },
  { title: 'Nome', value: 'nome' },
  { title: 'Telefone', value: 'telefone' },
  { title: 'E-mail', value: 'email' },
  { title: 'Data Cadastro', value: 'dataCadastro' },
  { title: 'Status', value: 'excluido' },
  { title: 'Ações', value: 'actions', sortable: false }
]

const pagina            = ref(1)
const itensPorPagina    = ref(5)

const formTitulo = computed(() =>
  editedItem.value.id ? 'Editar' : 'Cadastrar'
)

const confirmDialog = ref(false)
const itemParaExcluir = ref(null)

onMounted(loadClientes)

watch([pagina, itensPorPagina], () => {
  loadClientes(pagina.value, itensPorPagina.value)
})

onMounted(() => loadClientes(pagina.value, itensPorPagina.value))

function abrirDialog(item) {
  if (item) {
    editedItem.value = { ...item }
  } else {
    editedItem.value = {
      tipoPessoa: tiposPessoaOptions[0].value,
      nome: '',
      cpfCnpj: '',
      telefone: '',
      email: '',
      observacao: ''
    }
  }
  dialog.value = true
}

async function gravar() {
  await saveCliente(editedItem.value)
  toast.success('Sucesso!')
  dialog.value = false
  loadClientes()
}

function editItem(item) {
  abrirDialog(item)
}

function promptDelete(item) {
  itemParaExcluir.value = item
  confirmDialog.value = true
}

async function deleteItemConfirmado() {
  await deleteCliente(itemParaExcluir.value.id)
  toast.success('Cliente excluído com sucesso!')
  loadClientes(pagina.value, itensPorPagina.value)
  confirmDialog.value = false
  itemParaExcluir.value = null
}

function formatDate(date) {
  return date ? new Date(date).toLocaleDateString('pt-BR') : ''
}

</script>

<style scoped>

</style>
