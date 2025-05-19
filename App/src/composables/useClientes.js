import { ref } from 'vue'
import clienteService from '@/services/cliente-service.js'
import Cliente from '@/models/Cliente.js'

export function useClientes() {
  const clientes = ref([])
  const loading = ref(false)
  const error = ref(null)
  const totalItens = ref(0)

  async function loadClientes(pagina = 1, tamanhoPagina = 5) {
    loading.value = true
    error.value = null

    try {
      const resp = await clienteService.obterTodos(pagina, tamanhoPagina)
      const envelope = resp.data
      totalItens.value = envelope.totalItens
      const listaRaw = Array.isArray(envelope.itens) ? envelope.itens : []
      clientes.value = listaRaw.map(u => new Cliente(u))
    } catch (err) {
      console.error('Erro ao carregar clientes', err)
      error.value = err
    } finally {
      loading.value = false
    }
  }

  async function saveCliente(cliente) {
    loading.value = true
    error.value   = null
    try {
      if (cliente.id) {
        await clienteService.atualizar(cliente)
      } else {
        await clienteService.cadastrar(cliente)
      }
    } catch (err) {
      console.error('Erro ao salvar cliente', err)
      error.value = err
      throw err
    } finally {
      loading.value = false
    }
  }
  
  async function deleteCliente(id) {
    loading.value = true
    error.value   = null
    try {
      await clienteService.deletar(id)
    } catch (err) {
      console.error('Erro ao excluir cliente', err)
      error.value = err
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    clientes,
    loading,
    error,
    totalItens,
    loadClientes,
    saveCliente,
    deleteCliente
  }
}
