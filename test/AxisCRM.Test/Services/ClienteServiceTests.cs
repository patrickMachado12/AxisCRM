using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Classes;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.DTO.Cliente;
using AxisCRM.Api.DTO;
using AxisCRM.Api.Domain.Helper;
using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Test.Services
{
    public class ClienteServiceTests
    {
        private readonly ClienteService _service;
        private readonly Mock<IClienteRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public ClienteServiceTests()
        {
            _repositoryMock = new Mock<IClienteRepository>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _service = new ClienteService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Adicionar_QuandoCpfCnpjJaExiste_LancaBadRequestException()
        {
            var request = new ClienteRequestDTO 
            { 
                Nome = "Cliente", 
                CpfCnpj = "11122233344",
                Telefone = "21 1234-5678",
                Email = "cliente@gmail.com",
                Observacao = "Dados no campo de observação 123456 !@#$%¨&*()_+",
                TipoPessoa = TipoPessoa.Juridica
            };

            var clienteExistente = new Cliente { Id = 1, CpfCnpj = request.CpfCnpj };
            _repositoryMock
                .Setup(r => r.ObterPorCpfCnpjAsync(request.CpfCnpj))
                .ReturnsAsync(clienteExistente);

            await Assert.ThrowsAsync<BadRequestException>(() => _service.Adicionar(request));
        }

        [Fact]
        public async Task Adicionar_QuandoDadosValidos_RetornaClienteResponseDTO()
        {
            var request = new ClienteRequestDTO { Nome = "Cliente", CpfCnpj = "11122233344" };
            var entity = new Cliente { Id = 1, Nome = request.Nome, CpfCnpj = request.CpfCnpj, DataCadastro = DateTime.Today };
            var responseDto = new ClienteResponseDTO { Id = entity.Id, Nome = entity.Nome, CpfCnpj = entity.CpfCnpj, DataCadastro = entity.DataCadastro };

            _repositoryMock
                .Setup(r => r.ObterPorCpfCnpjAsync(request.CpfCnpj))
                .ReturnsAsync((Cliente)null);
            _mapperMock
                .Setup(m => m.Map<Cliente>(request))
                .Returns(entity);
            _mapperMock
                .Setup(m => m.Map<ClienteResponseDTO>(entity))
                .Returns(responseDto);

            var result = await _service.Adicionar(request);

            Assert.Equal(responseDto, result);
            _repositoryMock.Verify(r => r.AdicionarAsync(entity), Times.Once);
        }

        [Fact]
        public async Task Atualizar_QuandoClienteNaoEncontrado_LancaNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Cliente)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Atualizar(1, new ClienteRequestDTO()));
        }

        [Fact]
        public async Task Atualizar_QuandoCpfCnpjConflito_LancaBadRequestException()
        {
            var request = new ClienteRequestDTO { Nome = "Cliente", CpfCnpj = "11122233344" };
            var existing = new Cliente { Id = 1, Nome = "Old", CpfCnpj = request.CpfCnpj };
            var other = new Cliente { Id = 2, Nome = "Outro", CpfCnpj = request.CpfCnpj };

            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(existing.Id))
                .ReturnsAsync(existing);
            _mapperMock
                .Setup(m => m.Map(request, existing));
            _repositoryMock
                .Setup(r => r.ObterPorCpfCnpjAsync(existing.CpfCnpj))
                .ReturnsAsync(other);

            await Assert.ThrowsAsync<BadRequestException>(() => _service.Atualizar(existing.Id, request));
        }

        [Fact]
        public async Task Atualizar_QuandoDadosValidos_RetornaClienteResponseDTO()
        {
            var request = new ClienteRequestDTO { Nome = "Novo", CpfCnpj = "11122233344" };
            var existing = new Cliente { Id = 1, Nome = "Old", CpfCnpj = request.CpfCnpj, DataCadastro = DateTime.Today };
            var responseDto = new ClienteResponseDTO { Id = existing.Id, Nome = request.Nome, CpfCnpj = request.CpfCnpj, DataCadastro = existing.DataCadastro };

            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(existing.Id))
                .ReturnsAsync(existing);
            _mapperMock
                .Setup(m => m.Map(request, existing))
                .Callback<ClienteRequestDTO, Cliente>((req, ent) =>
                {
                    ent.Nome = req.Nome;
                    ent.CpfCnpj = req.CpfCnpj;
                });
            _repositoryMock
                .Setup(r => r.ObterPorCpfCnpjAsync(existing.CpfCnpj))
                .ReturnsAsync(existing);
            _repositoryMock
                .Setup(r => r.AtualizarAsync(existing))
                .ReturnsAsync(existing);
            _mapperMock
                .Setup(m => m.Map<ClienteResponseDTO>(existing))
                .Returns(responseDto);

            var result = await _service.Atualizar(existing.Id, request);

            Assert.Equal(responseDto, result);
            _repositoryMock.Verify(r => r.AtualizarAsync(existing), Times.Once);
        }

        [Fact]
        public async Task Excluir_QuandoClienteNaoEncontrado_LancaNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Cliente)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Excluir(1));
        }

        [Fact]
        public async Task Excluir_QuandoJaExcluido_LancaBadRequestException()
        {
            var cliente = new Cliente { Id = 1, Excluido = true };
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(cliente.Id))
                .ReturnsAsync(cliente);

            await Assert.ThrowsAsync<BadRequestException>(() => _service.Excluir(cliente.Id));
        }

        [Fact]
        public async Task Excluir_QuandoDadosValidos_RetornaClienteResponseDTO()
        {
            var cliente = new Cliente { Id = 1, Excluido = false, CpfCnpj = "11122233344", DataCadastro = DateTime.Today };
            var responseDto = new ClienteResponseDTO { Id = cliente.Id, Excluido = true };

            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(cliente.Id))
                .ReturnsAsync(cliente);
            _repositoryMock
                .Setup(r => r.AtualizarAsync(It.Is<Cliente>(c => c.Excluido)))
                .ReturnsAsync(cliente);
            _mapperMock
                .Setup(m => m.Map<ClienteResponseDTO>(It.IsAny<Cliente>()))
                .Returns(responseDto);

            var result = await _service.Excluir(cliente.Id);

            Assert.Equal(responseDto, result);
            _repositoryMock.Verify(r => r.AtualizarAsync(It.Is<Cliente>(c => c.Excluido)), Times.Once);
        }

        [Fact]
        public async Task ObterPorCpfCnpj_QuandoNaoEncontrado_LancaNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.ObterPorCpfCnpjAsync(It.IsAny<string>()))
                .ReturnsAsync((Cliente)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterPorCpfCnpj("999"));
        }

        [Fact]
        public async Task ObterPorCpfCnpj_QuandoEncontrado_RetornaClienteResponseDTO()
        {
            var cliente = new Cliente { Id = 1, Nome = "Cliente", CpfCnpj = "111" };
            var responseDto = new ClienteResponseDTO { Id = cliente.Id, Nome = cliente.Nome, CpfCnpj = cliente.CpfCnpj };

            _repositoryMock
                .Setup(r => r.ObterPorCpfCnpjAsync(cliente.CpfCnpj))
                .ReturnsAsync(cliente);
            _mapperMock
                .Setup(m => m.Map<ClienteResponseDTO>(cliente))
                .Returns(responseDto);

            var result = await _service.ObterPorCpfCnpj(cliente.CpfCnpj);

            Assert.Equal(responseDto, result);
        }

        [Fact]
        public async Task ObterPorId_QuandoNaoEncontrado_LancaNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Cliente)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterPorId(1));
        }

        [Fact]
        public async Task ObterPorId_QuandoEncontrado_RetornaClienteResponseDTO()
        {
            var cliente = new Cliente { Id = 1, Nome = "Cliente" };
            var responseDto = new ClienteResponseDTO { Id = cliente.Id, Nome = cliente.Nome };

            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(cliente.Id))
                .ReturnsAsync(cliente);
            _mapperMock
                .Setup(m => m.Map<ClienteResponseDTO>(cliente))
                .Returns(responseDto);

            var result = await _service.ObterPorId(cliente.Id);

            Assert.Equal(responseDto, result);
        }

        [Fact]
        public async Task ObterTodos_QuandoChamada_RetornaPaginacao()
        {
            var paginacao = new PaginacaoRequestDTO { Pagina = 2, TamanhoPagina = 5 };
            var clientes = new List<Cliente>
            {
                new Cliente { Id = 1 },
                new Cliente { Id = 2 }
            };
            int totalItens = 12;
            var dtos = new List<ClienteResponseDTO>
            {
                new ClienteResponseDTO { Id = 1 },
                new ClienteResponseDTO { Id = 2 }
            };

            _repositoryMock
                .Setup(r => r.ObterPaginadoAsync(paginacao.Pagina, paginacao.TamanhoPagina))
                .ReturnsAsync((clientes, totalItens));
            _mapperMock
                .Setup(m => m.Map<IEnumerable<ClienteResponseDTO>>(clientes))
                .Returns(dtos);

            var result = await _service.ObterTodos(paginacao);

            Assert.Equal(dtos, result.Itens);
            Assert.Equal(totalItens, result.TotalItens);
            Assert.Equal(paginacao.Pagina, result.PaginaAtual);
            Assert.Equal(paginacao.TamanhoPagina, result.TamanhoPagina);
            Assert.Equal((int)Math.Ceiling((double)totalItens / paginacao.TamanhoPagina), result.TotalPaginas);
        }

        [Fact]
        public async Task ObterTodos_QuandoTamanhoMaiorQueMax_TruncaTamanhoPagina()
        {
            var paginacao = new PaginacaoRequestDTO { Pagina = 1, TamanhoPagina = 200 };
            var clientes = new List<Cliente> { new Cliente { Id = 1 } };
            int totalItens = 50;
            var dtos = new List<ClienteResponseDTO> { new ClienteResponseDTO { Id = 1 } };

            _repositoryMock
                .Setup(r => r.ObterPaginadoAsync(paginacao.Pagina, paginacao.TamanhoPagina))
                .ReturnsAsync((clientes, totalItens));
            _mapperMock
                .Setup(m => m.Map<IEnumerable<ClienteResponseDTO>>(clientes))
                .Returns(dtos);

            var result = await _service.ObterTodos(paginacao);
            var expectedSize = 100;
            var expectedTotalPages = (int)Math.Ceiling((double)totalItens / expectedSize);

            Assert.Equal(dtos, result.Itens);
            Assert.Equal(expectedSize, result.TamanhoPagina);
            Assert.Equal(expectedTotalPages, result.TotalPaginas);
        }
    }
}