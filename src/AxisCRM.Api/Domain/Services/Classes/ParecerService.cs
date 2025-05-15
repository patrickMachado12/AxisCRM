using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.Domain.Services.Classes
{
    public class ParecerService : IParecerService
    {
        private const int TAMANO_MAXIMO_PAGINA = 100;
        private readonly IParecerRepository _parecerRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAtendimentoRepository _atendimentoRepository;
        private readonly IMapper _mapper;

        public ParecerService(IParecerRepository parecerRepository,
            IUsuarioRepository usuarioRepository,
            IAtendimentoRepository atendimentoRepository,
            IMapper mapper)
        {
            _parecerRepository = parecerRepository;
            _usuarioRepository = usuarioRepository;
            _atendimentoRepository = atendimentoRepository;
            _mapper = mapper;
        }

        public async Task<ParecerResponseDTO> Adicionar(ParecerRequestDTO entidade)
        {
            if (await _usuarioRepository.ObterPorIdAsync(entidade.IdUsuario) == null)
                throw new BadRequestException($"Usuário com id {entidade.IdUsuario} não encontrado. Verifique!");

             if (await _atendimentoRepository.ObterPorIdAsync(entidade.IdAtendimento) == null)
                throw new BadRequestException($"Atendimento com id {entidade.IdAtendimento} não encontrado. Verifique!");

            var entity = _mapper.Map<Parecer>(entidade);
            entity.DataCadastro = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;

            await _parecerRepository.AdicionarAsync(entity);

            return _mapper.Map<ParecerResponseDTO>(entity);
        }

        public async Task<ParecerResponseDTO> Atualizar(int id, ParecerRequestDTO entidade)
        {
            var existente = await _parecerRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException("Parecer não encontrado para atualização.");

            _mapper.Map(entidade, existente);
            existente.DataUltimaAlteracao = DateTime.Now;

            await _parecerRepository.AtualizarAsync(existente);

            return _mapper.Map<ParecerResponseDTO>(existente);
        }

        public async Task<ParecerResponseDTO> Excluir(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ParecerResponseDTO> ObterPorId(int id)
        {
            var parecer = await _parecerRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException("Parecer não encontrado.");
                
            return _mapper.Map<ParecerResponseDTO>(parecer);
        }

        public async Task<PaginacaoResponseDTO<ParecerResponseDTO>> ObterTodos(PaginacaoRequestDTO paginacao)
        {
            var tamanhoValido = Math.Min(paginacao.TamanhoPagina, TAMANO_MAXIMO_PAGINA);

            (IEnumerable<Parecer> pareceres, int totalItens) =
                await _parecerRepository.ObterPaginadoAsync(
                    paginacao.Pagina,
                    paginacao.TamanhoPagina
                );

            var pareceresDTO = _mapper.Map<IEnumerable<ParecerResponseDTO>>(pareceres);

            return new PaginacaoResponseDTO<ParecerResponseDTO>
            {
                Itens        = pareceresDTO,
                TotalItens   = totalItens,
                PaginaAtual  = paginacao.Pagina,
                TamanhoPagina= tamanhoValido,
                TotalPaginas = (int)Math.Ceiling((double)totalItens / tamanhoValido)
            };
        }
    }
}