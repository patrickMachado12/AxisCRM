export default class Parecer {
  constructor({
    id = 0,
    descricao = '',
    dataCadastro = null,
    pessoaContato = '',
    status = 0,
    idAtendimento = 0,
    idUsuario = 0,
    dataUltimaAlteracao = null
  } = {}) {
    this.id = id;
    this.descricao = descricao;
    this.dataCadastro = dataCadastro
      ? new Date(dataCadastro)
      : null;
    this.pessoaContato = pessoaContato;
    this.status = status;
    this.idAtendimento = idAtendimento;
    this.idUsuario = idUsuario;
    this.dataUltimaAlteracao = dataUltimaAlteracao
      ? new Date(dataUltimaAlteracao)
      : null;
  }
}
