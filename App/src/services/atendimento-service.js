import api from "./api";

export async function obterAtendimentoFiltrado(
  idUsuario = null,
  idCliente = null,
  status    = null,
  dataInicial = null,
  dataFinal   = null
) {
  const params = {};
  if (idCliente != null) params.idCliente  = idCliente;
  if (status != null) params.status = status;
  if (idUsuario != null) params.idUsuario = idUsuario;
  if (dataInicial) params.dataInicial = dataInicial;
  if (dataFinal) params.dataFinal = dataFinal;

  const { data } = await api.get("/atendimentos", { params });
  return data;
}
export async function obterAtendimentoPorId(id) {
  const { data } = await api.get(`/atendimentos/${id}`);
  return data;
}

export async function cadastrarAtendimento(atendimento) {
  const { data } = await api.post("/atendimentos", atendimento);
  return data;
}

export async function atualizarAtendimento(atendimento) {
  const { data } = await api.put(
    `/atendimentos/${atendimento.id}`,
    atendimento
  );
  return data;
}

export async function alterarStatusAtendimento(id) {
  const { data } = await api.patch(`/atendimentos/${id}`);
  return data;
}

export default {
  obterAtendimentoFiltrado,
  obterAtendimentoPorId,
  cadastrarAtendimento,
  atualizarAtendimento,
  alterarStatusAtendimento,
};
