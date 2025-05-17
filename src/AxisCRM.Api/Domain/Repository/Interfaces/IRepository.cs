using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.Domain.Repository.Interfaces
{
    /// <summary>
    /// Interface genérica para criação de repositórios do tipo CRUD.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <typeparam name="Id">Tipo do Id</typeparam>
    public interface IRepository<T, Id> where T : class
    {
        Task<IEnumerable<T>> ObterAsync();
        Task<T> ObterPorIdAsync(Id id);
        Task<T> AdicionarAsync(T entidade);
        Task<T> AtualizarAsync(T entidade);
        Task ExcluirAsync(T entidade);
        Task<(IEnumerable<T> entidades, int TotalItens)> ObterPaginadoAsync(
            int pagina,
            int tamanhoPagina
        );
    }
}