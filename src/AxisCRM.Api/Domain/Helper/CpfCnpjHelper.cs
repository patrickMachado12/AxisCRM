using System.Text.RegularExpressions;
using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Api.Domain.Helper
{
    public static class CpfCnpjHelper
    {
        /// <summary>
        /// Retorna true se o CPF ou CNPJ estiver em formato válido, de acordo com o tipo de pessoa.
        /// </summary>
        public static bool IsValid(TipoPessoa? tipoPessoa, string cpfCnpj)
        {
            if (!tipoPessoa.HasValue || string.IsNullOrWhiteSpace(cpfCnpj))
                return false;

            switch (tipoPessoa.Value)
            {
                case TipoPessoa.Fisica:
                    // Formato esperado: 000.000.000-00
                    return Regex.IsMatch(cpfCnpj, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$");
                case TipoPessoa.Juridica:
                    // Formato esperado: 00.000.000/0000-00
                    return Regex.IsMatch(cpfCnpj, @"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$");
                default:
                    return false;
            }
        }
    }
}
