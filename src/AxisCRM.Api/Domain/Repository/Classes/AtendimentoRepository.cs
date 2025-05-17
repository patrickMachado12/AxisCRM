using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Data;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AxisCRM.Api.Domain.Repository.Classes
{
    public class AtendimentoRepository : IAtendimentoRepository
    {
        private readonly ApplicationContext _contexto;
        
		public AtendimentoRepository(ApplicationContext context)
		{
			_contexto = context ?? throw new ArgumentNullException(nameof(context));
		}

        public async Task<Atendimento> AdicionarAsync(Atendimento entidade)
        {
            await _contexto.Atendimento.AddAsync(entidade);
			await _contexto.SaveChangesAsync();

			return entidade;
        }

        public async Task<Atendimento> AtualizarAsync(Atendimento entidade)
        {
            var entidadeBanco = await _contexto.Atendimento
                .FirstOrDefaultAsync(u => u.Id == entidade.Id);

            if (entidadeBanco is null)
                throw new NotFoundException($"Atendimento com id {entidade.Id} não foi encontrado.");

            _contexto.Entry(entidadeBanco).CurrentValues.SetValues(entidade);
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
            return await _contexto.Atendimento
                .AsNoTracking()
                .Include(a => a.Cliente)
                .Include(a => a.Usuario)
                .Include(a => a.Pareceres)
                .ThenInclude(p => p.Usuario)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Atendimento>> ObterAtendimentosFiltrados(
            int idUsuario,
            int idCliente,
            StatusAtendimento status,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            return await _contexto.Atendimento
                .AsNoTracking()
                .Include(a => a.Cliente)
                .Include(a => a.Usuario)
                .Include(a => a.Pareceres)
                    .ThenInclude(p => p.Usuario)
                .Where(a =>
                    a.IdUsuario == idUsuario &&
                    a.IdCliente == idCliente &&
                    a.Status == status &&
                    a.DataCadastro >= dataInicial &&
                    a.DataCadastro <= dataFinal
                )
                .OrderBy(a => a.Id)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Atendimento> entidades, int TotalItens)> ObterPaginadoAsync(
            int pagina, 
            int tamanhoPagina
        )
        {
            var query = _contexto.Atendimento
                .AsNoTracking()
                .Include(a => a.Cliente)
                .Include(a => a.Usuario)
                .Include(a => a.Pareceres)
                    .ThenInclude(p => p.Usuario);

            var totalItens = await query.CountAsync();

            var atendimentos = await query
                .OrderBy(u => u.Id)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (atendimentos, totalItens);
        }

        public async Task<Atendimento> ObterPorIdAsync(int id)
        {
            var atendimento = await _contexto.Atendimento
                .AsNoTracking()
                .Include(a => a.Cliente)
                .Include(a => a.Usuario)
                .Include(a => a.Pareceres)
                    .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (atendimento is null)
                throw new NotFoundException($"Atendimento com id {id} não foi encontrado.");

            return atendimento;
        }
    }
}