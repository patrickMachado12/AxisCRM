using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Models;

namespace AxisCRM.Api.Domain.Repository.Interfaces
{
    public interface IParecerRepository : IRepository<Parecer, int>
    {
        Task<(IEnumerable<Parecer> Pareceres, int TotalItens)> ObterPaginadoAsync(
            int pagina,
            int tamanhoPagina
        );
    }
}