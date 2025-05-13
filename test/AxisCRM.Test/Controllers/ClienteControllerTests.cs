using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AxisCRM.Api.Controllers;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.DTO.Cliente;
using AxisCRM.Api.DTO;
using AxisCRM.Api.Domain.Validator;
using AxisCRM.Api.Domain.Validator.ClienteValidator;
using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Test.Controllers
{
    public class ClienteControllerTests
    {
        private readonly ClienteController _controller;
        private readonly Mock<IClienteService> _serviceMock;
        private readonly IValidator<ClienteRequestDTO> _clienteValidator;

        public ClienteControllerTests()
        {
            _serviceMock = new Mock<IClienteService>();
            _clienteValidator = new ClienteValidador();
            _controller = new ClienteController(
                _serviceMock.Object,
                _clienteValidator
            );
        }

        [Fact]
        public async Task Adicionar_ComDadosInvalidos_RetornaBadRequest()
        {
            var dto = new ClienteRequestDTO { Nome = "", CpfCnpj = "" };
            var result = await _controller.Adicionar(dto);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Adicionar_ComDadosValidos_RetornaOk()
        {
            var dto = new ClienteRequestDTO 
            { 
                Nome = "Cliente", 
                CpfCnpj = "84.482.291/0001-91",
                Telefone = "21 1234-5678",
                Email = "cliente@gmail.com",
                Observacao = "Dados no campo de observação 123456 !@#$%¨&*()_+",
                TipoPessoa = TipoPessoa.Juridica,
            };

            var response = new ClienteResponseDTO 
            { 
                Id = 1, 
                Nome = dto.Nome, 
                CpfCnpj = dto.CpfCnpj,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Observacao = dto.Observacao, 
                DataCadastro = DateTime.Today 
            };

            _serviceMock
                .Setup(s => s.Adicionar(dto))
                .ReturnsAsync(response);

            var result = await _controller.Adicionar(dto);

            var ok    = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<ClienteResponseDTO>(ok.Value);

            Assert.Equal(response.Id, value.Id);
            Assert.Equal(response.Nome, value.Nome);
            Assert.Equal(response.CpfCnpj, value.CpfCnpj);
            Assert.Equal(response.Telefone, value.Telefone);
            Assert.Equal(response.Email, value.Email);
            Assert.Equal(response.Observacao,  value.Observacao);
            Assert.Equal(response.DataCadastro, value.DataCadastro);
        }

        [Fact]
        public async Task ObterPorId_ComIdInvalido_RetornaNotFound()
        {
            _serviceMock
                .Setup(s => s.ObterPorId(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException("Não encontrado"));

            var result = await _controller.ObterPorId(99);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ObterPorId_ComIdValido_RetornaOk()
        {
            var dto = new ClienteResponseDTO { Id = 1, Nome = "Cliente", CpfCnpj = "84.482.291/0001-91" };
            _serviceMock
                .Setup(s => s.ObterPorId(dto.Id))
                .ReturnsAsync(dto);

            var result = await _controller.ObterPorId(dto.Id);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<ClienteResponseDTO>(ok.Value);

            Assert.Equal(dto.Id, value.Id);
            Assert.Equal(dto.Nome, value.Nome);
            Assert.Equal(dto.CpfCnpj, value.CpfCnpj);
        }

        [Fact]
        public async Task ObterTodos_RetornaOkComPaginacao()
        {
            var page = new PaginacaoRequestDTO { Pagina = 1, TamanhoPagina = 5 };
            var listDto = new PaginacaoResponseDTO<ClienteResponseDTO>
            {
                Itens = new List<ClienteResponseDTO>
                {
                    new ClienteResponseDTO { Id = 1, Nome = "Cliente1", CpfCnpj = "111" }
                },
                TotalItens = 1
            };
            _serviceMock
                .Setup(s => s.ObterTodos(page))
                .ReturnsAsync(listDto);

            var result = await _controller.ObterTodos(page);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<PaginacaoResponseDTO<ClienteResponseDTO>>(ok.Value);

            Assert.Single(value.Itens);
            Assert.Equal(listDto.TotalItens, value.TotalItens);
        }

        [Fact]
        public async Task Atualizar_ComDadosInvalidos_RetornaBadRequest()
        {
            var dto = new ClienteRequestDTO { Nome = "", CpfCnpj = "" };
            var result = await _controller.Atualizar(1, dto);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Atualizar_ComIdInexistente_RetornaNotFound()
        {
            var dto = new ClienteRequestDTO 
            { 
                Nome = "Cliente", 
                CpfCnpj = "84.482.291/0001-91",
                Telefone = "21 1234-5678",
                Email = "cliente@gmail.com",
                Observacao = "Dados no campo de observação 123456 !@#$%¨&*()_+",
                TipoPessoa = TipoPessoa.Juridica,
            };

            _serviceMock
                .Setup(s => s.Atualizar(1, dto))
                .ThrowsAsync(new NotFoundException("Usuário não encontrado"));

            var result = await _controller.Atualizar(1, dto);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Atualizar_ComDadosValidos_RetornaOk()
        {
            var dto = new ClienteRequestDTO 
            { 
                Nome = "Cliente", 
                CpfCnpj = "84.482.291/0001-91",
                Telefone = "21 1234-5678",
                Email = "cliente@gmail.com",
                Observacao = "Dados no campo de observação 123456 !@#$%¨&*()_+",
                TipoPessoa = TipoPessoa.Juridica,
            };

            var response = new ClienteResponseDTO 
            { 
                Id = 1, 
                Nome = dto.Nome, 
                CpfCnpj = dto.CpfCnpj,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Observacao = dto.Observacao, 
                DataCadastro = DateTime.Today 
            };

            _serviceMock
                .Setup(s => s.Atualizar(1, dto))
                .ReturnsAsync(response);

            var result = await _controller.Atualizar(1, dto);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<ClienteResponseDTO>(ok.Value);

            Assert.Equal(response.Id, value.Id);
            Assert.Equal(response.Nome, value.Nome);
            Assert.Equal(response.CpfCnpj, value.CpfCnpj);
            Assert.Equal(response.Telefone, value.Telefone);
            Assert.Equal(response.Email, value.Email);
            Assert.Equal(response.Observacao, value.Observacao);
            Assert.Equal(response.DataCadastro, value.DataCadastro);
        }

        [Fact]
        public async Task Excluir_ComIdInexistente_RetornaNotFound()
        {
            _serviceMock
                .Setup(s => s.Excluir(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException("Não encontrado"));

            var result = await _controller.Excluir(99);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Excluir_ComIdValido_RetornaOk()
        {
            _serviceMock
                .Setup(s => s.Excluir(1))
                .ReturnsAsync(new ClienteResponseDTO { Id = 1, Nome = "Cliente", CpfCnpj = "111" });

            var result = await _controller.Excluir(1);
            Assert.IsType<OkResult>(result.Result);
        }
    }
}
