using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Services.Exceptions;

namespace AxisCRM.Api.Domain.Helper
{
    public static class RegraNegocio
    {
        /// <summary>
        /// Recebe uma lista de (condição, mensagem) e, se alguma falhar, lança BadRequestException com todas as mensagens.
        /// </summary>
        public static void Validate(params (bool Condition, string ErrorMessage)[] rules)
        {
            var failures = rules
                .Where(r => !r.Condition)
                .Select(r => r.ErrorMessage)
                .ToArray();

            if (failures.Any())
                throw new BadRequestException(string.Join(" | ", failures));
        }
    }
}