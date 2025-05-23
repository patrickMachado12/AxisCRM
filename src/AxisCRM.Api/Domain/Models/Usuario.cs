using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Api.Domain.Models
{
    public class Usuario : EntidadeBase
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Excluido { get; set; } = false;
        public DateTime? DataExclusao { get; set; }
        public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Padrao;
    }
}