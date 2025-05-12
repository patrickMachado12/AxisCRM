using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Cliente;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AxisCRM.Api.Controllers
{
    [ApiController]
    [Route("clientes")]
    public class ClienteController : BaseController
    {
        private readonly IClienteService _clienteService;
        private readonly IValidator<ClienteRequestDTO> _clienteValidator;

        public ClienteController(IClienteService clienteService, IValidator<ClienteRequestDTO> clienteValidator)
        {
            _clienteService = clienteService;
            _clienteValidator = clienteValidator;
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Adiciona um novo cliente.", 
            Description = "Este endpoint adiciona um novo cliente no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClienteResponseDTO>> Adicionar(ClienteRequestDTO dto)
        {
            var validacao = _clienteValidator.Validate(dto);

            if (!validacao.IsValid)
                return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_clienteService.Adicionar(dto), false);
        }
        
        [HttpGet]
        [SwaggerOperation(
            Summary = "Obtém uma lista de clientes.", 
            Description = "Este endpoint lista todos os clientes cadastrados no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginacaoResponseDTO<ClienteResponseDTO>>> ObterTodos([FromQuery] PaginacaoRequestDTO paginacao)
        {
            return await ProcessarTarefa(_clienteService.ObterTodos(paginacao));
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Obtém um cliente por identificador específico.", 
            Description = "Este endpoint retorna os dados de um cliente específico com base no ID."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClienteResponseDTO>> ObterPorId(int id)
        {
            return await ProcessarTarefa(_clienteService.ObterPorId(id));
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um cliente existente.", 
            Description = "Este endpoint atualiza os dados de um cliente existente no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClienteResponseDTO>> Atualizar(int id, ClienteRequestDTO dto)
        {
           var validacao = _clienteValidator.Validate(dto);
           
           if (!validacao.IsValid)
              return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_clienteService.Atualizar(id, dto));
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Exclui um cliente.", 
            Description = "Este endpoint exclui um cliente do sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClienteResponseDTO>> Excluir(int id)
        {
            var resultado = await ProcessarTarefa(_clienteService.Excluir(id));

            if (resultado.Result is ObjectResult erro &&
                erro.StatusCode != StatusCodes.Status200OK)
            {
                return erro;
            }

            return Ok();
        }
    }
}