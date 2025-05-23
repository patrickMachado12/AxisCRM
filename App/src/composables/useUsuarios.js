import { ref } from 'vue'
import usuarioService from '@/services/usuario-service.js'
import Usuario from '@/models/Usuario.js'

export function useUsuarios() {
  const usuarios = ref([])
  const loading = ref(false)
  const error = ref(null)
  const totalItens = ref(0)

  async function loadUsuarios(pagina = 1, tamanhoPagina = 5) {
    loading.value = true
    error.value   = null

    try {
      const resp     = await usuarioService.obterTodos(pagina, tamanhoPagina)
      const envelope = resp.data
      totalItens.value = envelope.totalItens
      const listaRaw = Array.isArray(envelope.itens) ? envelope.itens : []
      usuarios.value   = listaRaw.map(u => new Usuario(u))
    } catch (err) {
      console.error('Erro ao carregar usuários', err)
      error.value = err
    } finally {
      loading.value = false
    }
  }

  async function saveUsuario(usuario) {
    loading.value = true
    error.value   = null
    try {
      if (usuario.id) {
        await usuarioService.atualizar(usuario)
      } else {
        await usuarioService.cadastrar(usuario)
      }
    } catch (err) {
      console.error('Erro ao salvar usuário', err)
      error.value = err
      throw err
    } finally {
      loading.value = false
    }
  }
  
  async function deleteUsuario(id) {
    loading.value = true
    error.value   = null
    try {
      await usuarioService.deletar(id)
    } catch (err) {
      console.error('Erro ao excluir usuário', err)
      error.value = err
      throw err
    } finally {
      loading.value = false
    }
  }


  return {
    usuarios,
    loading,
    error,
    totalItens,
    loadUsuarios,
    saveUsuario,
    deleteUsuario
  }
}
