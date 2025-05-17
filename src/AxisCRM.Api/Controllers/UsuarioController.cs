using FluentValidation;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AxisCRM.Api.Domain.Validator;

namespace AxisCRM.Api.Controllers
{
    [ApiController]
    [Route("usuarios")]
    public class UsuarioController : BaseController
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UsuarioCadastroValidador  _usuarioCadastroValidator;
        private readonly UsuarioEdicaoValidador _usuarioEdicaoValidator;
        private readonly IValidator<UsuarioLoginRequestDTO> _usuarioLoginValidator;

        public UsuarioController(
            IUsuarioService usuarioService, 
            UsuarioCadastroValidador usuarioCadastroValidator,
            UsuarioEdicaoValidador usuarioEdicaoValidator,
            IValidator<UsuarioLoginRequestDTO> usuarioLoginValidator)
        {
            _usuarioService = usuarioService;
            _usuarioCadastroValidator = usuarioCadastroValidator;
            _usuarioEdicaoValidator = usuarioEdicaoValidator;
            _usuarioLoginValidator = usuarioLoginValidator;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Efetua a autenticação do usuário no sistema.", 
            Description = "Este endpoint retorna o token de autenticação do usuário."
        )]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioLoginResponseDTO>> Autenticar(UsuarioLoginRequestDTO dto)
        {   
            var validacao = _usuarioLoginValidator.Validate(dto);

            if (!validacao.IsValid)
                return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_usuarioService.Autenticar(dto), false);
        }

        /// <param name="dto"> 
        /// Perfis de usuário:
        ///     1 = Administrador |
        ///     2 = Padrão |
        ///     3 = Moderador |
        /// </param>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [SwaggerOperation(
            Summary = "Adiciona um novo usuário.", 
            Description = "Este endpoint adiciona um novo usuário no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioResponseDTO>> Adicionar(UsuarioRequestDTO dto)
        {
            var validacao = _usuarioCadastroValidator.Validate(dto);

            if (!validacao.IsValid)
                return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));
                
            return await ProcessarTarefa(_usuarioService.Adicionar(dto));
        }

        [HttpGet]
        [Route("")]
        [Authorize(Policy = "Admin")]
        [SwaggerOperation(
            Summary = "Obtém uma lista de usuários.", 
            Description = "Este endpoint retorna uma lista de usuários do sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginacaoResponseDTO<UsuarioResponseDTO>>> ObterTodos([FromQuery] PaginacaoRequestDTO paginacao)
        {
            return await ProcessarTarefa(_usuarioService.ObterTodos(paginacao));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = "Admin")]
        [SwaggerOperation(
            Summary = "Obtém um usuário por identificador específico.", 
            Description = "Este endpoint retorna os dados de um usuário específico com base no ID."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioResponseDTO>> ObterPorId(int id)
        {
            return await ProcessarTarefa(_usuarioService.ObterPorId(id));
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = "Admin")]
        [SwaggerOperation(
            Summary = "Atualiza um usuário existente.", 
            Description = "Este endpoint atualiza os dados de um usuário existente no sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioResponseDTO>> Atualizar(int id, UsuarioRequestDTO dto)
        {
           var validacao = _usuarioEdicaoValidator.Validate(dto);
           
           if (!validacao.IsValid)
              return BadRequest(validacao.Errors.Select(e => new { e.ErrorMessage }));

            return await ProcessarTarefa(_usuarioService.Atualizar(id, dto));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "Admin")]
        [SwaggerOperation(
            Summary = "Exclui um usuário.", 
            Description = "Este endpoint exclui um usuário do sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioResponseDTO>> Excluir(int id)
        {
            var resultado = await ProcessarTarefa(_usuarioService.Excluir(id));

            if (resultado.Result is ObjectResult erro &&
                erro.StatusCode != StatusCodes.Status200OK)
            {
                return erro;
            }

            return Ok();
        }

    }
}