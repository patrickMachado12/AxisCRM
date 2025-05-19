import api from "./api";

export async function login(email, senha) {
  const { data } = await api.post("/usuarios/login", { email, senha });
  return data;
}

export async function logout() {
  const { data } = await api.delete("/logout");
  return data;
}

export async function cadastrar(usuario) {
  const { data } = await api.post("/usuarios", usuario);
  return data;
}

export async function atualizar(usuario) {
  const { data } = await api.put(`/usuarios/${usuario.id}`, usuario);
  return data;
}

export async function deletar(id) {
  const { data } = await api.delete(`/usuarios/${id}`);
  return data;
}

export async function obterTodos(pagina, tamanhoPagina) {
  return await api.get("/usuarios", {
    params: { pagina, tamanhoPagina }
  });
}

export async function obterPorId(id) {
  const { data } = await api.get(`/usuarios/${id}`);
  return data;
}

export default {
  login,
  logout,
  cadastrar,
  atualizar,
  deletar,
  obterTodos,
  obterPorId,
};
