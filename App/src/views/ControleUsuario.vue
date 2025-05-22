<template>
  <v-container fluid>
    <v-row class="d-flex align-center mb-4">
      <v-col cols="12">
        <v-card-title>Cadastro de Usuários</v-card-title>
        <v-divider />
      </v-col>
      <v-col cols="12">
        <v-btn 
          id="btn-cadastrar" 
          color="primary" 
          type="button" 
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
              <v-col cols="12" sm="12">
                <v-text-field
                  v-model="editedItem.email"
                  label="E-mail"
                  required
                />
              </v-col>
              <v-col cols="12" sm="12" md="6">
                <v-text-field
                  type="password"
                  v-model="editedItem.senha"
                  label="Senha"
                  :rules="editedItem.id ? [] : [v => !!v || 'Senha é obrigatória']"
                  :required="!editedItem.id"
                />
              </v-col>
              <v-col cols="12" sm="12" md="6">
                <v-select
                  v-model="editedItem.perfil"
                  :items="perfisOptions"
                  item-title="label"
                  item-value="value"
                  label="Perfil"
                  required
                />
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn id="btn-gravar" color="primary" @click="gravar()">
            Gravar
          </v-btn>
          <v-btn text @click="dialog = false">
            Fechar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-row>
      <v-col cols="12">
        <v-data-table-server
          :headers="headers"
          :items="usuarios"
          :loading="loading"
          v-model:page="pagina"
          v-model:items-per-page="itensPorPagina"
          :items-length="totalItens"
          class="elevation-8"
        >
          <template #item.perfil="{ item }">
            {{ perfisOptions.find(p => p.value === item.perfil)?.label }}
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
          <strong>{{ itemParaExcluir?.email }}</strong>?
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn 
            id="btn-confirm-delete" 
            color="primary"
            variant="outlined"
            @click="deleteItemConfirmado"
          >
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
import { useUsuarios } from '@/composables/useUsuarios'
import { toast } from 'vue3-toastify'
import 'vue3-toastify/dist/index.css'

const { 
  usuarios, 
  loading, 
  totalItens, 
  loadUsuarios, 
  saveUsuario, 
  deleteUsuario 
} = useUsuarios()

const dialog = ref(false)
const editedItem = ref({})

const { smAndDown } = useDisplay()

const perfisOptions = [
  { label: 'Admin', value: 1 },
  { label: 'Padrão', value: 2 },
  { label: 'Moderador', value: 3 },
]

const statusOptions = [
  { label: 'Ativo', value: false },
  { label: 'Excluído', value: true },
]

const headers = [
  { title: 'Código', value: 'id', align: 'start', sortable: true },
  { title: 'E-mail', value: 'email' },
  { title: 'Perfil', value: 'perfil' },
  { title: 'Data Cadastro', value: 'dataCadastro' },
  { title: 'Status', value: 'excluido' },
  { title: 'Ações', value: 'actions', sortable: false },
]

const pagina = ref(1)
const itensPorPagina = ref(5)

const formTitulo = computed(() =>
  editedItem.value.id ? 'Editar' : 'Cadastrar'
)

const confirmDialog = ref(false)
const itemParaExcluir = ref(null)

watch([pagina, itensPorPagina], () => {
  loadUsuarios(pagina.value, itensPorPagina.value)
})

onMounted(() => loadUsuarios(pagina.value, itensPorPagina.value))

function abrirDialog(item) {
  if (item) {
    editedItem.value = { ...item }
  } else {
    editedItem.value = {
      email: '',
      senha: '',
      perfil: perfisOptions[0].value
    }
  }
  dialog.value = true
}

async function gravar() {
  const payload = {
    email: editedItem.value.email,
    perfil: editedItem.value.perfil,

    ...(editedItem.value.senha ? { senha: editedItem.value.senha } : {}),

    ...(editedItem.value.id ? { id: editedItem.value.id } : {})
  }
  await saveUsuario(payload)
  toast.success('Sucesso!')
  dialog.value = false
  loadUsuarios(pagina.value, itensPorPagina.value)
}

function editItem(item) {
  abrirDialog(item)
}

function promptDelete(item) {
  itemParaExcluir.value = item
  confirmDialog.value = true
}

async function deleteItemConfirmado() {
  await deleteUsuario(itemParaExcluir.value.id)
  toast.success('Usuário excluído com sucesso!')
  loadUsuarios(pagina.value, itensPorPagina.value)
  confirmDialog.value = false
  itemParaExcluir.value = null
}

function formatDate(date) {
  return date ? new Date(date).toLocaleDateString('pt-BR') : ''
}
</script>

<style scoped>

</style>
