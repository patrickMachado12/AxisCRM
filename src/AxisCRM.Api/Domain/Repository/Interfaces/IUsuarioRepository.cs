using AxisCRM.Api.Domain.Models;

namespace AxisCRM.Api.Domain.Repository.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario, int>
    {
        Task<Usuario> ObterPorEmailAsync(string email);
    }
}