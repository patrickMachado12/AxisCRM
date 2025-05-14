using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.DTO.Atendimento
{
    public class AtendimentoResponseDTO
    {
        public int Id { get; set; }
        public string Assunto { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Descricao { get; set; }
        public int IdCliente { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
        public int IdUsuario { get; set; }
        public bool StatusEncerrado { get; set; }
        public DateTime? DataEncerramento { get; set; }
    }
}