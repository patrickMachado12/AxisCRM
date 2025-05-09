using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Models;

namespace AxisCRM.Api.Domain.Repository.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario, int>
    {
        Task<Usuario> ObterPorEmailAsync(string email);
        Task<(IEnumerable<Usuario> Usuarios, int TotalItens)> ObterPaginadoAsync(
            int pagina,
            int tamanhoPagina
        );
    }
}