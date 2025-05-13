using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Data;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AxisCRM.Api.Domain.Repository.Classes
{
    public class AtendimentoRepository : IAtendimentoRepository
    {
        private readonly ApplicationContext _contexto;
        
		public AtendimentoRepository(ApplicationContext context)
		{
			_contexto = context;
		}

        public async Task<Atendimento> AdicionarAsync(Atendimento entidade)
        {
            await _contexto.Atendimento.AddAsync(entidade);
			await _contexto.SaveChangesAsync();

			return entidade;
        }

        public async Task<Atendimento> AtualizarAsync(Atendimento entidade)
        {
            Atendimento? entidadeBanco = await _contexto.Atendimento
                                                        .Where(u => u.Id == entidade.Id)
                                                        .FirstOrDefaultAsync();

            _contexto.Entry(entidadeBanco).CurrentValues.SetValues(entidade);
            _contexto.Update<Atendimento>(entidadeBanco);

            await _contexto.SaveChangesAsync();

            return entidadeBanco;
        }

        public Task ExcluirAsync(Atendimento entidade)
        {
            _contexto.Update<Atendimento>(entidade);
            return _contexto.SaveChangesAsync();
        }

        public async Task<IEnumerable<Atendimento>> ObterAsync()
        {
            return await _contexto.Atendimento.AsNoTracking()
													.OrderBy(u => u.Id)
													.ToListAsync();
        }

        public async Task<(IEnumerable<Atendimento> Atendimentos, int TotalItens)> ObterPaginadoAsync(int pagina, int tamanhoPagina)
        {
            var query = _contexto.Atendimento.AsQueryable();
            var totalItens = await query.CountAsync();

            query = query.OrderBy(u => u.Id);

            var atendimentos = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (atendimentos, totalItens);
        }

        public async Task<Atendimento> ObterPorIdAsync(int id)
        {
            return await _contexto.Atendimento.Where(u => u.Id == id)
                                                    .FirstOrDefaultAsync();
        }
    }
}