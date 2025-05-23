using System.Security.Claims;

namespace AxisCRM.Api.Domain.Extensoes
{
    public static class HttpContextAccessorExtensao
    {
        public static string ObterEmailUsuarioLogado(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Email)?
                    .Value ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
        }
    }
}