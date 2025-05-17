using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AxisCRM.Api.Data;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AxisCRM.Api.Domain.Repository.Classes
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationContext _contexto;

        public UsuarioRepository(ApplicationContext context)
        {
            _contexto = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Usuario> AdicionarAsync(Usuario entidade)
        {
            await _contexto.Usuario.AddAsync(entidade);
            await _contexto.SaveChangesAsync();

            return entidade;
        }

        public async Task<Usuario> AtualizarAsync(Usuario entidade)
        {
            var entidadeBanco = await _contexto.Usuario
                .FirstOrDefaultAsync(u => u.Id == entidade.Id);

            if (entidadeBanco is null)
                throw new NotFoundException($"Usuario com id {entidade.Id} não foi encontrado.");

            _contexto.Entry(entidadeBanco).CurrentValues.SetValues(entidade);
            await _contexto.SaveChangesAsync();

            return entidadeBanco;
        }

        public Task ExcluirAsync(Usuario entidade)
        {
            _contexto.Update<Usuario>(entidade);
            return _contexto.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> ObterAsync()
        {
            return await _contexto.Usuario.AsNoTracking()
                                            .OrderBy(u => u.Id)
                                            .ToListAsync();
        }

        public async Task<Usuario> ObterPorIdAsync(int id)
        {
            var usuario = await _contexto.Usuario.Where(u => u.Id == id)
                                                .FirstOrDefaultAsync();
            if (usuario is null)
                throw new NotFoundException($"Usuario com id {id} não foi encontrado.");

            return usuario;     
        }

        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            var usuario = await _contexto.Usuario.AsNoTracking()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (usuario is null)
                throw new NotFoundException($"Usuario com e-mail {email} não foi encontrado.");

            return usuario; 
        }

        public async Task<(IEnumerable<Usuario> entidades, int TotalItens)> ObterPaginadoAsync(
            int pagina,
            int tamanhoPagina
        )
        {
            var query = _contexto.Usuario.AsQueryable();
            var totalItens = await query.CountAsync();

            query = query.OrderBy(u => u.Id);

            var usuarios = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (usuarios, totalItens);
        }
    }
}