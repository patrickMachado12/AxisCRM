import api from "./api";

export async function obterPorId(idAtendimento, idParecer) {
  const { data } = await api.get(
    `/atendimentos/${idAtendimento}/pareceres/${idParecer}`
  );
  return data;
}

export async function cadastrar(idAtendimento, parecer) {
  const { data } = await api.post(
    `/atendimentos/${idAtendimento}/pareceres`,
    parecer
  );
  return data;
}

export async function atualizar(idAtendimento, idParecer, parecer) {
  const { data } = await api.put(
    `/atendimentos/${idAtendimento}/pareceres/${idParecer}`,
    parecer
  );
  return data;
}

export default {
  obterPorId,
  cadastrar,
  atualizar,
};
