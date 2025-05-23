export default class Usuario {
  constructor({
    id = 0,
    email = '',
    senha = '',
    perfil = null,
    dataCadastro = null,
    excluido = false,
    dataExclusao = null
  } = {}) {
    this.id = id;
    this.email = email;
    this.senha = senha;
    this.perfil = perfil;
    this.dataCadastro = dataCadastro
      ? new Date(dataCadastro)
      : null;
    this.excluido = excluido;
    this.dataExclusao = dataExclusao
      ? new Date(dataExclusao)
      : null;
  }

  modeloValidoLogin() {
    return Boolean(this.email && this.senha);
  }
}
