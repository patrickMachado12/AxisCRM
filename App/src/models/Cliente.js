export default class Cliente {
  constructor({
    id = 0,
    nome = '',
    tipoPessoa = null,
    cpfCnpj = '',
    telefone = '',
    email = '',
    observacao = null,
    dataCadastro = null,
    excluido = false,
    dataExclusao = null
  } = {}) {
    this.id = id;
    this.nome = nome;
    this.tipoPessoa = tipoPessoa;
    this.cpfCnpj = cpfCnpj;
    this.telefone = telefone;
    this.email = email;
    this.observacao = observacao;
    this.dataCadastro = dataCadastro ? new Date(dataCadastro) : null;
    this.excluido = excluido;
    this.dataExclusao = dataExclusao ? new Date(dataExclusao) : null;
  }
}
