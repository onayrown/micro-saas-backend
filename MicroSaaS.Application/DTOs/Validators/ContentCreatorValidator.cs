using FluentValidation;

namespace MicroSaaS.Application.DTOs.Validators;

public class ContentCreatorValidator : AbstractValidator<ContentCreatorDto>
{
    public ContentCreatorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(2).WithMessage("Nome deve ter no mínimo 2 caracteres")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório")
            .EmailAddress().WithMessage("E-mail inválido");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Nome de usuário é obrigatório")
            .MinimumLength(3).WithMessage("Nome de usuário deve ter no mínimo 3 caracteres")
            .MaximumLength(50).WithMessage("Nome de usuário deve ter no máximo 50 caracteres")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Nome de usuário só pode conter letras, números e underscores");
    }
}
