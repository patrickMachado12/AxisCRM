import Parecer from './Parecer';

export default class Atendimento {
  constructor({
    id = 0,
    assunto = '',
    idCliente = 0,
    dataCadastro = null,
    dataEncerramento = null,
    status = 0,
    dataUltimaAtualizacao = null,
    idUsuario = 0,
    pareceres = [],
    historico = ''
  } = {}) {
    this.id = id;
    this.assunto = assunto;
    this.idCliente = idCliente;
    this.dataCadastro = dataCadastro
      ? new Date(dataCadastro)
      : null;
    this.dataEncerramento = dataEncerramento
      ? new Date(dataEncerramento)
      : null;
    this.status = status;
    this.dataUltimaAtualizacao = dataUltimaAtualizacao
      ? new Date(dataUltimaAtualizacao)
      : null;
    this.idUsuario = idUsuario;
    this.pareceres = Array.isArray(pareceres)
      ? pareceres.map(p => new Parecer(p))
      : [];
    this.historico = historico;
  }
}
