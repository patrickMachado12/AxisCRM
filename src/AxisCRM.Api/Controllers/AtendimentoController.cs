using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Atendimento;
using AxisCRM.Api.DTO.Parecer;
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
        private readonly IValidator<AtendimentoEdicaoRequestDTO> _atendimentoEdicaoValidator;
        private readonly IParecerService _parecerService;
        private readonly IValidator<ParecerRequestDTO> _parecerValidator;
        private readonly IValidator<ParecerEdicaoRequestDTO> _parecerEdicaoValidator;

        public AtendimentoController(
            IAtendimentoService atendimentoService,
            IValidator<AtendimentoRequestDTO> atendimentoValidator,
            IValidator<AtendimentoEdicaoRequestDTO> atendimentoEdicaoValidator,
            IParecerService parecerService,
            IValidator<ParecerRequestDTO> parecerValidator,
            IValidator<ParecerEdicaoRequestDTO> parecerEdicaoValidator)
        {
            _atendimentoService = atendimentoService;
            _atendimentoValidator = atendimentoValidator;
            _atendimentoEdicaoValidator = atendimentoEdicaoValidator;
            _parecerService = parecerService;
            _parecerValidator = parecerValidator;
            _parecerEdicaoValidator = parecerEdicaoValidator;
        }

        #region Atendimento
        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Adiciona um novo atendimento.",
            Description = "Este endpoint adiciona um novo atendimento no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AtendimentoResponseDTO>> AdicionarAtendimento(AtendimentoRequestDTO dto)
        {
            // TODO: Verificar a possibilidade de adicionar essa questão do IdUsuario do token em uma função separada e isolada. Algo como token mesmo.
            var idUsuario = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            dto.IdUsuario = Convert.ToInt32(idUsuario);
            dto.Parecer.IdUsuario = Convert.ToInt32(idUsuario);
            var validacaoAtendimento = _atendimentoValidator.Validate(dto);

            if (!validacaoAtendimento.IsValid)
                return BadRequest(validacaoAtendimento.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_atendimentoService.Adicionar(dto), true);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Obtém uma lista de atendimentos filtrados.",
            Description = "Este endpoint lista os atendimentos conforme o filtro realizado."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AtendimentoResponseDTO>>> ObterAtendimentosFiltrados(
            [FromQuery] int idUsuario,
            int idCliente,
            StatusAtendimento status,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            return await ProcessarTarefa(_atendimentoService.ObterAtendimentosFiltrados(
                idUsuario,
                idCliente,
                status,
                dataInicial,
                dataFinal));
        }

        [HttpGet]
        [Authorize]
        [Route("{idAtendimento}")]
        [SwaggerOperation(
            Summary = "Obtém um atendimento por identificador específico.", 
            Description = "Este endpoint retorna os dados de um atendimento específico com base no ID."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AtendimentoResponseDTO>> ObterPorId(int idAtendimento)
        {
            return await ProcessarTarefa(_atendimentoService.ObterPorId(idAtendimento));
        }

        [HttpPut]
        [Authorize]
        [Route("{idAtendimento}")]
        [SwaggerOperation(
            Summary = "Atualiza um atendimento existente.",
            Description = "Este endpoint atualiza os dados de um atendimento existente no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AtendimentoResponseDTO>> AtualizarAtendimento(
            [FromRoute] int idAtendimento,
            [FromBody] AtendimentoEdicaoRequestDTO dto)
        {
            var validacaoEdicaoAtendimento = _atendimentoEdicaoValidator.Validate(dto);

            if (!validacaoEdicaoAtendimento.IsValid)
                return BadRequest(validacaoEdicaoAtendimento.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_atendimentoService.AtualizarAtendimento(idAtendimento, dto));
        }

        [HttpPatch]
        [Authorize]
        [Route("{idAtendimento}")]
        [SwaggerOperation(
            Summary = "Altera o status de um atendimento.",
            Description = "Este endpoint finaliza um atendimento no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AtendimentoResponseDTO>> AlterarStatus(int idAtendimento, AtendimentoEdicaoStatusRequestDTO dto)
        {
            var resultado = await ProcessarTarefa(_atendimentoService.AlterarStatus(idAtendimento, dto));

            if (resultado.Result is ObjectResult erro &&
                erro.StatusCode != StatusCodes.Status200OK)
            {
                return erro;
            }

            return Ok();
        }
        #endregion
        
        #region Parecer
        [HttpPost("{idAtendimento}/pareceres")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Adiciona um novo parecer a um atendimento.", 
            Description = "Este endpoint adiciona um novo parecer a um atendimento do sistema."
        )]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ParecerResponseDTO>> AdicionarParecer(
            [FromRoute] int idAtendimento,
            [FromBody] ParecerRequestDTO dto)
        {
            dto.IdAtendimento = idAtendimento;
            
            var idUsuario = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            dto.IdUsuario = Convert.ToInt32(idUsuario);

            var validacaoParecer = _parecerValidator.Validate(dto);

            if (!validacaoParecer.IsValid)
                return BadRequest(validacaoParecer.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_parecerService.AdicionarParecer(dto), true);
        }

        [HttpGet]
        [Authorize]
        [Route("{idAtendimento}/pareceres/{idParecer}")]
        [SwaggerOperation(
            Summary = "Obtém um parecer por identificador específico vinculado ao atendimento.", 
            Description = "Este endpoint retorna os dados de um parecer específico com base no identificador do parecer e do atendimento."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParecerResponseDTO>> ObterParecerPorId(
            [FromRoute] int idAtendimento,
            [FromRoute] int idParecer)
        {
            return await ProcessarTarefa(_parecerService.ObterParecerPorId(idAtendimento, idParecer));
        }

        [HttpPut]
        [Authorize]
        [Route("{idAtendimento}/pareceres/{idParecer}")]
        [SwaggerOperation(
            Summary = "Atualiza um parecer existente.", 
            Description = "Este endpoint atualiza os dados de um parecer existente no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParecerResponseDTO>> AtualizarParecer(
            [FromRoute] int idAtendimento,
            [FromRoute] int idParecer,
            [FromBody] ParecerEdicaoRequestDTO dto)
        {
            var validacaoEdicaoParecer = _parecerEdicaoValidator.Validate(dto);
            
            if (!validacaoEdicaoParecer.IsValid)
                return BadRequest(validacaoEdicaoParecer.Errors.Select(e => new { e.ErrorMessage }));

                return await ProcessarTarefa(_parecerService.AtualizarParecer(idAtendimento, idParecer, dto));
        }
        #endregion
    }
}