using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.DTO.Atendimento
{
    public class AtendimentoResponseDTO
    {
        public int Id { get; set; }
        public string Assunto { get; set; }
        public int IdCliente { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public StatusAtendimento Status { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        public int IdUsuario { get; set; }
        public List<ParecerResponseDTO> Pareceres { get; set; } = new();
        public string Historico { get; set; }
    }
}