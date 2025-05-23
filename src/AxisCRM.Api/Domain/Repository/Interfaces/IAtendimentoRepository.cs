using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Models;

namespace AxisCRM.Api.Domain.Repository.Interfaces
{
    public interface IAtendimentoRepository : IRepository<Atendimento, int>
    {
        Task<IEnumerable<Atendimento>> ObterAtendimentosFiltrados(
            int? idUsuario,
            int? idCliente,
            StatusAtendimento status,
            DateTime? dataInicial,
            DateTime? dataFinal);
    }
}