using System;
using System.Threading.Tasks;
using AxisCRM.Api.Controllers;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AxisCRM.Tests.Controllers
{
    public class BaseControllerTests
    {
        private readonly FakeController _controller;

        public BaseControllerTests()
        {
            _controller = new FakeController();
        }

        [Fact]
        public async Task ProcessarTarefa_DeveRetornarCreated_QuandoNovoCadastro()
        {
            var esperado = "Novo usuário";
            Task<string> tarefa = Task.FromResult(esperado);

            var resultado = await _controller.ExecutarProcessarTarefa(tarefa, true);

            var createdResult = Assert.IsType<CreatedResult>(resultado.Result);
            Assert.Equal(esperado, createdResult.Value);
        }

        [Fact]
        public async Task ProcessarTarefa_DeveRetornarOk_QuandoNaoEhNovoCadastro()
        {
            var esperado = "Login bem-sucedido";
            Task<string> tarefa = Task.FromResult(esperado);

            var resultado = await _controller.ExecutarProcessarTarefa(tarefa);

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(esperado, okResult.Value);
        }

        [Fact]
        public async Task ProcessarTarefa_DeveRetornarNotFound_QuandoChamarNotFoundException()
        {
            Task<string> tarefa = Task.FromException<string>(new NotFoundException("Usuário não encontrado"));

            var resultado = await _controller.ExecutarProcessarTarefa(tarefa);
            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            var erro = Assert.IsType<ModelErrorDTO>(notFoundResult.Value);
            Assert.Equal(404, erro.StatusCode);
        }

        [Fact]
        public async Task ProcessarTarefa_DeveRetornarBadRequest_QuandoChamarBadRequestException()
        {
            Task<string> tarefa = Task.FromException<string>(new BadRequestException("Dados inválidos"));

            var resultado = await _controller.ExecutarProcessarTarefa(tarefa);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var erro = Assert.IsType<ModelErrorDTO>(badRequestResult.Value);
            Assert.Equal(400, erro.StatusCode);
        }

        [Fact]
        public async Task ProcessarTarefa_DeveRetornarUnauthorized_QuandoUnauthorizedAccessException()
        {
            Task<string> tarefa = Task.FromException<string>(new UnauthorizedAccessException("Acesso negado"));

            var resultado = await _controller.ExecutarProcessarTarefa(tarefa);

            var authenticationResult = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
            var erro = Assert.IsType<ModelErrorDTO>(authenticationResult.Value);
            Assert.Equal(403, erro.StatusCode);
        }

        [Fact]
        public async Task ProcessarTarefa_DeveRetornarUnauthorized_QuandoUnauthorizedException()
        {
            Task<string> tarefa = Task.FromException<string>(new UnauthorizedException("Sem autorização"));

            var resultado = await _controller.ExecutarProcessarTarefa(tarefa);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
            var erro = Assert.IsType<ModelErrorDTO>(unauthorizedResult.Value);
            Assert.Equal(401, erro.StatusCode);
        }

        [Fact]
        public async Task ProcessarTarefa_DeveRetornarInternalServerError_QuandoException()
        {
            Task<string> tarefa = Task.FromException<string>(new Exception("Erro interno"));

            var resultado = await _controller.ExecutarProcessarTarefa(tarefa);

            var errorResult = Assert.IsType<ObjectResult>(resultado.Result);
            var erro = Assert.IsType<ModelErrorDTO>(errorResult.Value);
            Assert.Equal(500, erro.StatusCode);
        }
    }

    public class FakeController : BaseController
    {
        public Task<ActionResult<T>> ExecutarProcessarTarefa<T>(Task<T> tarefa, bool isNovoCadastro = false)
        {
            return ProcessarTarefa(tarefa, isNovoCadastro);
        }
    }
}
