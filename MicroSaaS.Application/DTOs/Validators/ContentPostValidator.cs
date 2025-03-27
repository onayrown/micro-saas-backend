using FluentValidation;

namespace MicroSaaS.Application.DTOs.Validators;

public class ContentPostValidator : AbstractValidator<ContentPostDto>
{
    public ContentPostValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Conteúdo da postagem é obrigatório")
            .MaximumLength(1000).WithMessage("Conteúdo não pode exceder 1000 caracteres");

        RuleFor(x => x.Platform)
            .IsInEnum().WithMessage("Plataforma de mídia social inválida");

        RuleFor(x => x.ScheduledTime)
            .NotEmpty().WithMessage("Data de agendamento é obrigatória")
            .GreaterThan(DateTime.Now).WithMessage("A data de agendamento deve ser no futuro");
    }
}
