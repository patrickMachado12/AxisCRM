import api from "./api";

async function cadastrar(cliente) {
  const { data } = await api.post("/clientes", cliente);
  return data;
}

async function atualizar(cliente) {
  const { data } = await api.put(`/clientes/${cliente.id}`, cliente);
  return data;
}

async function deletar(id) {
  const { data } = await api.delete(`/clientes/${id}`);
  return data;
}

async function obterTodos(pagina, tamanhoPagina) {
  return await api.get("/clientes", {
    params: { pagina, tamanhoPagina }
  });
}

async function obterPorId(id) {
  const { data } = await api.get(`/clientes/${id}`);
  return data;
}

export default {
  cadastrar,
  atualizar,
  deletar,
  obterTodos,
  obterPorId,
};
