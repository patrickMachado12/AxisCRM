using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.DTO.Atendimento
{
    public class AtendimentoEdicaoStatusRequestDTO
    {
        public string Motivo { get; set; }
        public StatusAtendimento Status { get; set; }
    }
}