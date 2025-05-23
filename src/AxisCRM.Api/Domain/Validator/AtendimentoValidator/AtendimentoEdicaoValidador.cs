using AxisCRM.Api.DTO.Atendimento;
using FluentValidation;

namespace AxisCRM.Api.Domain.Validator.AtendimentoValidator
{
    public class AtendimentoEdicaoValidador : AbstractValidator<AtendimentoEdicaoRequestDTO>
    {
        public AtendimentoEdicaoValidador()
        {
            RuleFor(x => x)
                .Must(dto =>
                    !string.IsNullOrWhiteSpace(dto.Assunto) 
                    || dto.IdCliente.HasValue)
                .WithMessage("Informe ao menos o Assunto ou o Cliente para atualizar o atendimento.");
        }
    }
}