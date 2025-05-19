<template>
  <v-container fluid>
    <MensagemSucesso ref="successMessage" :message="message" />

    <v-row class="d-flex align-center mb-4">
      <v-col cols="12">
        <h2 class="titulo">Cadastro de Usuários</h2>
        <v-divider />
      </v-col>
      <v-col cols="12">
        <v-btn id="btn-cadastrar" dark type="button" @click="abrirDialog()">
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
          <v-btn id="btn-gravar" type="button" @click="gravar()">
            Gravar
          </v-btn>
          <v-btn text type="button" @click="dialog = false">
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
          class="elevation-1"
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
            <v-icon small @click="deleteItem(item)">
              mdi-delete
            </v-icon>
          </template>
        </v-data-table-server>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { useDisplay } from 'vuetify'
import { useUsuarios } from '@/composables/useUsuarios'
import MensagemSucesso from '@/components/alerts/MensagemSucesso.vue'

const { 
  usuarios, 
  loading, 
  totalItens, 
  loadUsuarios, 
  saveUsuario, 
  deleteUsuario 
} = useUsuarios()

// diálogo e estado
const dialog = ref(false)
const editedItem = ref({})
const message = ref('')

// responsividade
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

// título do formulário
const formTitulo = computed(() =>
  editedItem.value.id ? 'Editar' : 'Cadastrar'
)

// recarrega lista ao mudar página ou tamanho
watch([pagina, itensPorPagina], () => {
  loadUsuarios(pagina.value, itensPorPagina.value)
})

// chamada inicial
onMounted(() => loadUsuarios(pagina.value, itensPorPagina.value))

// abre diálogo para novo/edição
function abrirDialog(item) {
  if (item) {
    // edição: clona todos os campos
    editedItem.value = { ...item }
  } else {
    // cadastro: inicia campos vazios e perfil padrão
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
  message.value = 'Usuário salvo com sucesso!'
  dialog.value = false

  loadUsuarios(pagina.value, itensPorPagina.value)
}

function editItem(item) {
  abrirDialog(item)
}

async function deleteItem(item) {
  await deleteUsuario(item.id)
  message.value = 'Usuário excluído com sucesso!'
  loadUsuarios(pagina.value, itensPorPagina.value)
}

function formatDate(date) {
  return date ? new Date(date).toLocaleDateString('pt-BR') : ''
}
</script>

<style scoped>
#btn-cadastrar {
  background-color: var(--cor-primaria);
  margin-left: 12px;
}

#btn-gravar {
  background-color: var(--cor-primaria);
  color: #fff;
}

.v-btn {
  text-transform: none;
}

.titulo {
  margin-bottom: 8px;
}
</style>
