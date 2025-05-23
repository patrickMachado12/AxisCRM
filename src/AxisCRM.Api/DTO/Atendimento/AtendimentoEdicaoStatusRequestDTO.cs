using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Api.DTO.Atendimento
{
    public class AtendimentoEdicaoStatusRequestDTO
    {
        public string Motivo { get; set; }
        public StatusAtendimento Status { get; set; }
    }
}