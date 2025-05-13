using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Atendimento;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AxisCRM.Api.Controllers
{
    [ApiController]
    [Route("atendimentos")]
    public class AtendimentoController : BaseController
    {
        private readonly IAtendimentoService _atendimentoService;
        private readonly IValidator<AtendimentoRequestDTO> _atendimentoValidator;

        public AtendimentoController(IAtendimentoService atendimentoService, IValidator<AtendimentoRequestDTO> atendimentoValidator)
        {
            _atendimentoService = atendimentoService;
            _atendimentoValidator = atendimentoValidator;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Adiciona um novo atendimento.", 
            Description = "Este endpoint adiciona um novo atendimento no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AtendimentoResponseDTO>> Adicionar(AtendimentoRequestDTO dto)
        {
            var validacao = _atendimentoValidator.Validate(dto);

            if (!validacao.IsValid)
                return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_atendimentoService.Adicionar(dto), false);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Obtém uma lista de atendimentos.", 
            Description = "Este endpoint lista todos os atendimentos cadastrados no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginacaoResponseDTO<AtendimentoResponseDTO>>> ObterTodos([FromQuery] PaginacaoRequestDTO paginacao)
        {
            return await ProcessarTarefa(_atendimentoService.ObterTodos(paginacao));
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Obtém um atendimento por identificador específico.", 
            Description = "Este endpoint retorna os dados de um atendimento específico com base no ID."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AtendimentoResponseDTO>> ObterPorId(int id)
        {
            return await ProcessarTarefa(_atendimentoService.ObterPorId(id));
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um atendimento existente.", 
            Description = "Este endpoint atualiza os dados de um atendimento existente no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AtendimentoResponseDTO>> Atualizar(int id, AtendimentoRequestDTO dto)
        {
           var validacao = _atendimentoValidator.Validate(dto);
           
           if (!validacao.IsValid)
              return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_atendimentoService.Atualizar(id, dto));
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Encerrar um atendimento.", 
            Description = "Este endpoint encerra um atendimento do sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AtendimentoResponseDTO>> Encerrar(int id)
        {
            var resultado = await ProcessarTarefa(_atendimentoService.Excluir(id));

            if (resultado.Result is ObjectResult erro &&
                erro.StatusCode != StatusCodes.Status200OK)
            {
                return erro;
            }

            return Ok();
        }
    }
}