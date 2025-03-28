using FluentValidation;
using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Application.DTOs.ContentPost;

public class ContentPostValidator : AbstractValidator<CreatePostRequest>
{
    public ContentPostValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(280)
            .When(x => x.Platform == SocialMediaPlatform.Twitter)
            .MaximumLength(2200)
            .When(x => x.Platform == SocialMediaPlatform.LinkedIn);

        RuleFor(x => x.MediaUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.MediaUrl))
            .WithMessage("URL da mídia inválida");

        RuleFor(x => x.ScheduledFor)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ScheduledFor.HasValue)
            .WithMessage("A data de agendamento deve ser futura");
    }
}

public class UpdatePostValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(280)
            .When(x => x.Platform == SocialMediaPlatform.Twitter)
            .MaximumLength(2200)
            .When(x => x.Platform == SocialMediaPlatform.LinkedIn);

        RuleFor(x => x.MediaUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.MediaUrl))
            .WithMessage("URL da mídia inválida");

        RuleFor(x => x.ScheduledFor)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ScheduledFor.HasValue)
            .WithMessage("A data de agendamento deve ser futura");
    }
} 