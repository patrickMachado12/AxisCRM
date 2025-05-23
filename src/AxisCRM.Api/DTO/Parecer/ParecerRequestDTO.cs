using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Api.DTO.Parecer
{
    public class ParecerRequestDTO
    {
        public string Descricao { get; set; }
        public string PessoaContato { get; set; }
        public StatusAtendimento? Status { get; set; }
        public int IdUsuario { get; set; }
        public int IdAtendimento { get; set; }
    }
}