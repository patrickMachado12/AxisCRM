using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Enums;
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

        public async Task<ParecerResponseDTO> AdicionarParecer(ParecerRequestDTO entidade)
        {
            if (await _usuarioRepository.ObterPorIdAsync(entidade.IdUsuario) == null)
                throw new BadRequestException($"Usuário com id {entidade.IdUsuario} não encontrado. Verifique!");

            var atendimento = await _atendimentoRepository.ObterPorIdAsync(entidade.IdAtendimento);
            if (atendimento == null)
                throw new BadRequestException($"Atendimento com id {entidade.IdAtendimento} não encontrado. Verifique!");

            if (atendimento.Status == StatusAtendimento.Encerrado)
                throw new BadRequestException(
                    $"Não foi possível adicionar o parecer. O atendimento {entidade.IdAtendimento} já encontra-se encerrado.");

            if (entidade.Status.HasValue
                && atendimento.Status == StatusAtendimento.Reaberto
                && entidade.Status.Value == StatusAtendimento.Aberto)
            {
                throw new BadRequestException(
                    $"Não foi possível alterar o status para Aberto. O atendimento {entidade.IdAtendimento} já encontra-se reaberto.");
            }

            var parecer = _mapper.Map<Parecer>(entidade);
            parecer.DataCadastro = DateTime.Now;
            await _parecerRepository.AdicionarAsync(parecer);

            if (entidade.Status.HasValue)
            {
                atendimento.Status = entidade.Status.Value;
                atendimento.DataUltimaAtualizacao = DateTime.Now;

                if (entidade.Status.Value == StatusAtendimento.Encerrado)
                    atendimento.DataEncerramento = DateTime.Now;

                await _atendimentoRepository.AtualizarAsync(atendimento);
            }

            return _mapper.Map<ParecerResponseDTO>(parecer);
        }

        public async Task<ParecerResponseDTO> AtualizarParecer( int idAtendimento, int idParecer, ParecerEdicaoRequestDTO entidade)
        {
            var existente = await _parecerRepository.ObterPorIdAsync(idParecer)
                ?? throw new NotFoundException("Parecer não encontrado para atualização.");

            if (existente.IdAtendimento != idAtendimento)
                throw new BadRequestException("O parecer não pertence a este atendimento.");

            var atendimento = await _atendimentoRepository.ObterPorIdAsync(existente.IdAtendimento);
            if (atendimento.Status == StatusAtendimento.Encerrado)
                throw new BadRequestException("Não é permitido editar o parecer de um atendimento encerrado.");

            _mapper.Map(entidade, existente);
            existente.DataUltimaAlteracao = DateTime.Now;

            await _parecerRepository.AtualizarAsync(existente);

            return _mapper.Map<ParecerResponseDTO>(existente);
        }

        public async Task<ParecerResponseDTO> ObterParecerPorId(int idAtendimento, int idParecer)
        {
            var parecer = await _parecerRepository.ObterPorIdAsync(idParecer)
                ?? throw new NotFoundException("Parecer não encontrado para atualização.");

            if (parecer.IdAtendimento != idAtendimento)
                throw new BadRequestException("O parecer nao pertence a este atendimento.");

                return _mapper.Map<ParecerResponseDTO>(parecer);
        }

    }
}