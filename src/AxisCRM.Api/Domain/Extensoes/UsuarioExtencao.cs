using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Models;

namespace AxisCRM.Api.Domain.Extensoes
{
    public static class UsuarioExtencao
    {
        public static void Permitido(this Usuario usuario)
        {
            if (usuario.Perfil != PerfilUsuario.Admin)
                throw new UnauthorizedAccessException("Apenas administradores podem realizar esta operação.");
        }
    }
}