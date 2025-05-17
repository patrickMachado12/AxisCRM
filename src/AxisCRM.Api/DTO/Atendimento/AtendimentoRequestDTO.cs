using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.DTO.Atendimento
{
    public class AtendimentoRequestDTO
    {
        public string Assunto { get; set; }
        public int IdCliente { get; set; }
        public StatusAtendimento Status { get; set; }
        public ParecerRequestDTO Parecer { get; set; }
        public int IdUsuario { get; set; }
    }
}