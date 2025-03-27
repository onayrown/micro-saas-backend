using MicroSaaS.Shared.Enums;
using System.Text.RegularExpressions;

namespace MicroSaaS.Domain.Validators;

public static class ContentValidator
{
    private static readonly Dictionary<SocialMediaPlatform, (int MinLength, int MaxLength)> ContentLengthLimits = new()
    {
        { SocialMediaPlatform.Instagram, (1, 2200) },
        { SocialMediaPlatform.Facebook, (1, 63206) },
        { SocialMediaPlatform.Twitter, (1, 280) },
        { SocialMediaPlatform.LinkedIn, (1, 3000) }
    };

    private static readonly Regex UrlPattern = new(
        @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static (bool IsValid, string ErrorMessage) ValidateContent(
        string content,
        SocialMediaPlatform platform,
        DateTime? scheduledDate = null)
    {
        // Validar comprimento do conteúdo
        var (minLength, maxLength) = ContentLengthLimits[platform];
        if (string.IsNullOrWhiteSpace(content))
            return (false, "O conteúdo não pode estar vazio");

        if (content.Length < minLength)
            return (false, $"O conteúdo deve ter no mínimo {minLength} caracteres");

        if (content.Length > maxLength)
            return (false, $"O conteúdo deve ter no máximo {maxLength} caracteres");

        // Validar URLs no conteúdo
        var urls = ExtractUrls(content);
        foreach (var url in urls)
        {
            if (!UrlPattern.IsMatch(url))
                return (false, $"URL inválida encontrada: {url}");
        }

        // Validar data de agendamento
        if (scheduledDate.HasValue)
        {
            if (scheduledDate.Value < DateTime.UtcNow)
                return (false, "A data de agendamento não pode ser no passado");

            if (scheduledDate.Value > DateTime.UtcNow.AddDays(30))
                return (false, "A data de agendamento não pode ser superior a 30 dias no futuro");
        }

        return (true, string.Empty);
    }

    public static (bool IsValid, string ErrorMessage) ValidateMediaContent(
        string mediaUrl,
        SocialMediaPlatform platform)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl))
            return (false, "A URL da mídia não pode estar vazia");

        if (!UrlPattern.IsMatch(mediaUrl))
            return (false, "URL da mídia inválida");

        // Validar extensão do arquivo
        var allowedExtensions = GetAllowedExtensions(platform);
        var extension = Path.GetExtension(mediaUrl).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return (false, $"Extensão de arquivo não permitida para {platform}. Extensões permitidas: {string.Join(", ", allowedExtensions)}");

        return (true, string.Empty);
    }

    private static IEnumerable<string> ExtractUrls(string content)
    {
        var urlPattern = new Regex(@"https?:\/\/[^\s<>""']+|www\.[^\s<>""']+");
        return urlPattern.Matches(content).Select(m => m.Value);
    }

    private static string[] GetAllowedExtensions(SocialMediaPlatform platform)
    {
        return platform switch
        {
            SocialMediaPlatform.Instagram => new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4" },
            SocialMediaPlatform.Facebook => new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" },
            SocialMediaPlatform.Twitter => new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4" },
            SocialMediaPlatform.LinkedIn => new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4" },
            _ => Array.Empty<string>()
        };
    }
} 