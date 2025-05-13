using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.DTO.Atendimento
{
    public class AtendimentoRequestDTO
    {
        public string Assunto { get; set; }
        public int IdCliente { get; set; }
        public string Descricao { get; set; }
        public int IdUsuario { get; set; }
    }
}