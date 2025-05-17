using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Data;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AxisCRM.Api.Domain.Repository.Classes
{
    public class ParecerRepository : IParecerRepository
    {
        private readonly ApplicationContext _contexto;

        public ParecerRepository(ApplicationContext context)
        {
            _contexto = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Parecer> AdicionarAsync(Parecer entidade)
        {
            await _contexto.Parecer.AddAsync(entidade);
            await _contexto.SaveChangesAsync();

            return entidade;
        }

        public async Task<Parecer> AtualizarAsync(Parecer entidade)
        {
            var entidadeBanco = await _contexto.Parecer
                .FirstOrDefaultAsync(u => u.Id == entidade.Id);

            if (entidadeBanco is null)
                throw new NotFoundException($"Parecer com id {entidade.Id} não foi encontrado.");

            _contexto.Entry(entidadeBanco).CurrentValues.SetValues(entidade);
            await _contexto.SaveChangesAsync();

            return entidadeBanco;
        }

        public Task ExcluirAsync(Parecer entidade)
        {
            _contexto.Update<Parecer>(entidade);
            return _contexto.SaveChangesAsync();
        }

        public async Task<IEnumerable<Parecer>> ObterAsync()
        {
            return await _contexto.Parecer.AsNoTracking()
                                                    .OrderBy(u => u.Id)
                                                    .ToListAsync();
        }

        public async Task<(IEnumerable<Parecer> entidades, int TotalItens)> ObterPaginadoAsync(int pagina, int tamanhoPagina)
        {
            var query = _contexto.Parecer.AsQueryable();
            var totalItens = await query.CountAsync();

            query = query.OrderBy(u => u.Id);

            var pareceres = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (pareceres, totalItens);
        }

        public async Task<Parecer> ObterPorIdAsync(int id)
        {
            var parecer = await _contexto.Parecer.Where(u => u.Id == id)
                                                .FirstOrDefaultAsync();
            if (parecer is null)
                throw new NotFoundException($"Parecer com id {id} não foi encontrado.");

            return parecer;
        }
    }
}