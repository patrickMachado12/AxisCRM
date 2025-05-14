using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Parecer;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AxisCRM.Api.Controllers
{
    [ApiController]
    [Route("pareceres")]
    public class ParecerController : BaseController
    {
        private readonly IParecerService _parecerService;
        private readonly IValidator<ParecerRequestDTO> _parecerValidator;

        public ParecerController(IParecerService parecerService, IValidator<ParecerRequestDTO> parecerValidator)
        {
            _parecerService = parecerService;
            _parecerValidator = parecerValidator;
        }

        [HttpPost("{IdAtendimento}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Adiciona um novo parecer.", 
            Description = "Este endpoint adiciona um novo parecer no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ParecerResponseDTO>> Adicionar(
            [FromRoute] int IdAtendimento,
            [FromBody] ParecerRequestDTO dto)
        {
            dto.IdAtendimento = IdAtendimento;

            var validacao = _parecerValidator.Validate(dto);

            if (!validacao.IsValid)
                return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_parecerService.Adicionar(dto), false);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Obtém uma lista de parecers.", 
            Description = "Este endpoint lista todos os parecers cadastrados no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginacaoResponseDTO<ParecerResponseDTO>>> ObterTodos([FromQuery] PaginacaoRequestDTO paginacao)
        {
            return await ProcessarTarefa(_parecerService.ObterTodos(paginacao));
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Obtém um parecer por identificador específico.", 
            Description = "Este endpoint retorna os dados de um parecer específico com base no ID."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ParecerResponseDTO>> ObterPorId(int id)
        {
            return await ProcessarTarefa(_parecerService.ObterPorId(id));
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um parecer existente.", 
            Description = "Este endpoint atualiza os dados de um parecer existente no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ParecerResponseDTO>> Atualizar(int id, ParecerRequestDTO dto)
        {
           var validacao = _parecerValidator.Validate(dto);
           
           if (!validacao.IsValid)
              return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_parecerService.Atualizar(id, dto));
        }      
    }
}