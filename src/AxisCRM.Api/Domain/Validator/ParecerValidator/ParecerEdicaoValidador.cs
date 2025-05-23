using AxisCRM.Api.DTO.Parecer;
using FluentValidation;

namespace AxisCRM.Api.Domain.Validator.ParecerValidator
{
    public class ParecerEdicaoValidador : AbstractValidator<ParecerEdicaoRequestDTO>
    {
        public ParecerEdicaoValidador()
        {
            RuleFor(x => x)
                .Must(dto =>
                    !string.IsNullOrWhiteSpace(dto.Descricao) 
                    || !string.IsNullOrWhiteSpace(dto.PessoaContato)
                )
                .WithMessage("Informe ao menos a descrição ou a pessoa de contato para atualizar o parecer.");
        }
    }
}