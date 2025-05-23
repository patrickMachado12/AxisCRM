using AxisCRM.Api.DTO.Usuario;

namespace AxisCRM.Api.Domain.Services.Interfaces
{
    public interface IUsuarioService : IService<UsuarioRequestDTO, UsuarioResponseDTO, int>
    {
        Task<UsuarioLoginResponseDTO> Autenticar(UsuarioLoginRequestDTO usuarioLoginRequestDTO); 
        Task<UsuarioResponseDTO> ObterPorEmail(string email);
    }
}