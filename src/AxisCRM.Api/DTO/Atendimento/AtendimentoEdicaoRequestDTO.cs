using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.DTO.Atendimento
{
    public class AtendimentoEdicaoRequestDTO
    {
        public string Assunto { get; set; }
        public int IdCliente { get; set; }
    }
}