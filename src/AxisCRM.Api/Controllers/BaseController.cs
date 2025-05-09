using System.Security.Authentication;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AxisCRM.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Método genérico que executa qualquer task com tratamentos de exceções.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto de retorno</typeparam>
        /// <param name="tarefa">Task que deve ser executada.</param>
        /// <param name="isNovoCadastro">Indica se representa uma criação de recurso.</param>
        /// <returns>ActionResult com o resultado ou erro correspondente.</returns>
        protected async Task<ActionResult<T>> ProcessarTarefa<T>(Task<T> tarefa, bool isNovoCadastro = false)
        {
            try
            {
                return isNovoCadastro 
                    ? Created("", await tarefa)
                    : Ok(await tarefa);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ModelErrorDTO("Not found", 404, ex.Message));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ModelErrorDTO("Bad Request", 400, ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ModelErrorDTO("Forbidden", 403, ex.Message));
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new ModelErrorDTO("Unauthorized", 401, ex.Message));
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new ModelErrorDTO("Unauthorized", 401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ModelErrorDTO("Internal Server Error", 500, "Algo inesperado ocorreu. Erro: " + ex.Message));
            }
        }
    }
}