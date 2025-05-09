using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Api.DTO.Usuario
{
    public class UsuarioRequestDTO
    {
        public string Email { get; set; } 
        public string Senha { get; set; }
        public PerfilUsuario? Perfil { get; set; }
    }
}