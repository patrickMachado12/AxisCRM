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
using AxisCRM.Api.DTO.Atendimento;

namespace AxisCRM.Api.Domain.Services.Classes
{
    public class AtendimentoService : IAtendimentoService
    {
        private readonly IAtendimentoRepository _atendimentoRepository;
        private readonly IParecerService _parecerService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public AtendimentoService(
            IAtendimentoRepository atendimentoRepository,
            IParecerService parecerService,
            IUsuarioRepository usuarioRepository,
            IClienteRepository clienteRepository,
            IMapper mapper)
        {
            _atendimentoRepository = atendimentoRepository;
            _parecerService = parecerService;
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<AtendimentoResponseDTO> Adicionar(AtendimentoRequestDTO entidade)
        {
            // TODO:  Verificar para adicionar essas verificações em uma regra de negocio.
            // TODO:  Se tem mais de 2 Ifs, já é bom refatorar e por em uma função separada.

            if (await _clienteRepository.ObterPorIdAsync(entidade.IdCliente) == null)
                throw new BadRequestException($"Cliente com id {entidade.IdCliente} não encontrado. Verifique!");

            if (entidade.Status == StatusAtendimento.Reaberto)
                throw new BadRequestException("Não é permitido cadastrar um atendimento com status reaberto.");

            var entity = _mapper.Map<Atendimento>(entidade);
            entity.DataCadastro = DateTime.Now;
            if (entidade.Status == StatusAtendimento.Encerrado)
                entity.DataEncerramento = DateTime.Now;

            var atendimentoSalvo = await _atendimentoRepository.AdicionarAsync(entity);

            if (entidade.Parecer != null)
            {
                entidade.Parecer.IdAtendimento = atendimentoSalvo.Id;
                await _parecerService.AdicionarParecer(entidade.Parecer);
            }

            var atendimentoCompleto = await _atendimentoRepository.ObterPorIdAsync(atendimentoSalvo.Id);

            return _mapper.Map<AtendimentoResponseDTO>(atendimentoCompleto);
        }

        public async Task<AtendimentoResponseDTO> AlterarStatus(int id, AtendimentoEdicaoStatusRequestDTO RequestDTO)
        {
            // TODO:  Verificar para adicionar essas verificações em uma regra de negocio.
            // TODO:  Se tem mais de 2 Ifs, já é bom refatorar e por em uma função separada.
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(id);
            if (atendimento == null)
                throw new NotFoundException($"Atendimento {id} não encontrado.");

            if (RequestDTO.Status == StatusAtendimento.Aberto)
                throw new BadRequestException("Não é permitido alterar o status para 'Aberto'.");

            if (atendimento.Status == RequestDTO.Status)
            {
                var verbo = RequestDTO.Status == StatusAtendimento.Reaberto
                    ? "reabrir" 
                    : "encerrar";
                throw new BadRequestException($"Não é possível {verbo} um atendimento que já encontra-se {RequestDTO.Status}.");
            }

            atendimento.Status = RequestDTO.Status;

            //TODO: Aqui utilizei um ternário e a interpolação para criar o histórico.
            var acao = RequestDTO.Status == StatusAtendimento.Reaberto ? "Reaberto" : "Encerrado";
            var motivo = !string.IsNullOrWhiteSpace(RequestDTO.Motivo)
                        ? $" - Motivo: {RequestDTO.Motivo}"
                        : string.Empty;
            atendimento.Historico +=
                $"{DateTime.Now:dd/MM/yyyy HH:mm} - {acao}{motivo}{Environment.NewLine} ";

            atendimento.DataUltimaAtualizacao = DateTime.Now;
            atendimento.DataEncerramento = RequestDTO.Status == StatusAtendimento.Encerrado
                                 ? DateTime.Now
                                 : null;

            await _atendimentoRepository.AtualizarAsync(atendimento);

            return _mapper.Map<AtendimentoResponseDTO>(atendimento);
        }

        public async Task<AtendimentoResponseDTO> AtualizarAtendimento(int idAtendimento, AtendimentoEdicaoRequestDTO entidade)
        {
            var existente = await _atendimentoRepository.ObterPorIdAsync(idAtendimento)
                            ?? throw new NotFoundException("Atendimento não encontrado para atualização.");
            
            if (existente.Status == StatusAtendimento.Encerrado)
                throw new BadRequestException("Não é permitido atualizar um atendimento que esteja encerrado.");

            _mapper.Map(entidade, existente);
            existente.DataUltimaAtualizacao = DateTime.Now;

            await _atendimentoRepository.AtualizarAsync(existente);

            return _mapper.Map<AtendimentoResponseDTO>(existente);
        }

        public async Task<IEnumerable<AtendimentoResponseDTO>> ObterAtendimentosFiltrados(
            int idUsuario,
            int idCliente,
            StatusAtendimento status,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            //TODO: Utilizei uma função chamada "Pattern-matching" que foi implementada no C# 7.
            //TODO: Junto a ela, utilizei uma interpolação para retornar a mensagem de erro ao usuário.
            if (status is not (StatusAtendimento.Aberto
                   or StatusAtendimento.Encerrado
                   or StatusAtendimento.Reaberto))
            {
                throw new BadRequestException(
                    $"Status inválido: {(int)status}. " +
                    $"Use {(int)StatusAtendimento.Aberto} (Aberto), " +
                    $"{(int)StatusAtendimento.Encerrado} (Encerrado) ou " +
                    $"{(int)StatusAtendimento.Reaberto} (Reaberto).");
            }
                
            var atendimentos = await _atendimentoRepository
                .ObterAtendimentosFiltrados(idUsuario, idCliente, status, dataInicial, dataFinal);
            return _mapper.Map<IEnumerable<AtendimentoResponseDTO>>(atendimentos);
        }

        public async Task<AtendimentoResponseDTO> ObterPorId(int id)
        {
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException($"Atendimento {id} não foi encontrado");

            return _mapper.Map<AtendimentoResponseDTO>(atendimento);
        }
    }
}