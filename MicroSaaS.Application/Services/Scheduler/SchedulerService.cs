using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Services.Scheduler;

/// <summary>
/// Implementação do serviço de agendamento pertencente à camada de aplicação 
/// conforme os princípios da Clean Architecture.
/// </summary>
public class SchedulerService : BackgroundService, ISchedulerService
{
    private readonly ILogger<SchedulerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer _processingTimer;
    private Timer _notificationTimer;
    private bool _isRunning;

    public SchedulerService(
        ILogger<SchedulerService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _isRunning = false;
    }

    public async Task StartAsync()
    {
        if (_isRunning)
            return;

        _logger.LogInformation("Iniciando serviço de agendamento de publicações");
        
        // Timer para processar posts agendados (verifica a cada 1 minuto)
        _processingTimer = new Timer(
            async _ => await ProcessScheduledPostsAsync(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(1));

        // Timer para enviar notificações (verifica a cada 15 minutos)
        _notificationTimer = new Timer(
            async _ => await SendUpcomingPostNotificationsAsync(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(15));

        _isRunning = true;
    }

    public async Task StopAsync()
    {
        if (!_isRunning)
            return;

        _logger.LogInformation("Parando serviço de agendamento de publicações");
        
        _processingTimer?.Change(Timeout.Infinite, 0);
        _processingTimer?.Dispose();
        
        _notificationTimer?.Change(Timeout.Infinite, 0);
        _notificationTimer?.Dispose();

        _isRunning = false;
    }

    public async Task<ContentPost> SchedulePostAsync(ContentPost post)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        if (!post.ScheduledFor.HasValue)
            throw new ArgumentException("A data de agendamento é obrigatória");

        using var scope = _serviceProvider.CreateScope();
        var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();

        post.Status = PostStatus.Scheduled;
        post.UpdatedAt = DateTime.UtcNow;

        var updatedPost = await contentPostRepository.UpdateAsync(post);
        _logger.LogInformation("Post {PostId} agendado para {ScheduledTime}", post.Id, post.ScheduledFor);

        return updatedPost;
    }

    public async Task CancelScheduledPostAsync(Guid postId)
    {
        using var scope = _serviceProvider.CreateScope();
        var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();

        var post = await contentPostRepository.GetByIdAsync(postId);
        if (post == null || post.Status != PostStatus.Scheduled)
            return;

        post.Status = PostStatus.Draft;
        post.UpdatedAt = DateTime.UtcNow;

        await contentPostRepository.UpdateAsync(post);
        _logger.LogInformation("Agendamento do post {PostId} cancelado", postId);
    }

    public async Task ProcessScheduledPostsAsync()
    {
        try
        {
            _logger.LogDebug("Processando posts agendados");
            
            using var scope = _serviceProvider.CreateScope();
            var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();
            var socialMediaService = scope.ServiceProvider.GetRequiredService<ISocialMediaIntegrationService>();

            // Obter todos os posts agendados para agora ou no passado
            var now = DateTime.UtcNow;
            var posts = await contentPostRepository.GetByStatusAsync(PostStatus.Scheduled);
            var postsToPublish = posts.Where(p => p.ScheduledFor.HasValue && p.ScheduledFor.Value <= now).ToList();

            _logger.LogInformation("Encontrados {Count} posts para publicar", postsToPublish.Count);

            foreach (var post in postsToPublish)
            {
                try
                {
                    // Publicar o post na rede social
                    await socialMediaService.PostContentAsync(post);

                    // Atualizar status no banco de dados
                    post.Status = PostStatus.Published;
                    post.PublishedAt = DateTime.UtcNow;
                    post.UpdatedAt = DateTime.UtcNow;

                    await contentPostRepository.UpdateAsync(post);
                    _logger.LogInformation("Post {PostId} publicado com sucesso", post.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao publicar post {PostId}", post.Id);
                    
                    // Marcar como falha na publicação
                    post.Status = PostStatus.Failed;
                    post.UpdatedAt = DateTime.UtcNow;
                    await contentPostRepository.UpdateAsync(post);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar posts agendados");
        }
    }

    public async Task<IEnumerable<ContentPost>> GetScheduledPostsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        using var scope = _serviceProvider.CreateScope();
        var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();

        var posts = await contentPostRepository.GetByStatusAsync(PostStatus.Scheduled);
        return posts.Where(p => p.ScheduledFor.HasValue && 
                               p.ScheduledFor.Value >= startDate && 
                               p.ScheduledFor.Value <= endDate)
                   .OrderBy(p => p.ScheduledFor)
                   .ToList();
    }

    public async Task SendUpcomingPostNotificationsAsync(int hoursAhead = 1)
    {
        try
        {
            _logger.LogDebug("Verificando posts para enviar notificações");
            
            using var scope = _serviceProvider.CreateScope();
            var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();

            // Obter posts que serão publicados dentro do período especificado
            var now = DateTime.UtcNow;
            var notificationThreshold = now.AddHours(hoursAhead);
            
            var posts = await contentPostRepository.GetByStatusAsync(PostStatus.Scheduled);
            var postsForNotification = posts.Where(p => 
                p.ScheduledFor.HasValue && 
                p.ScheduledFor.Value > now && 
                p.ScheduledFor.Value <= notificationThreshold)
                .ToList();

            if (postsForNotification.Any())
            {
                _logger.LogInformation("Enviando notificações para {Count} posts agendados", postsForNotification.Count);
                
                // Enviar notificações (implementação real usaria um serviço de notificação)
                foreach (var post in postsForNotification)
                {
                    _logger.LogInformation("Notificação enviada para post {PostId} agendado para {ScheduledTime}", 
                        post.Id, post.ScheduledFor);
                }
            }
            else
            {
                _logger.LogDebug("Nenhum post encontrado para notificação");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificações de posts agendados");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Iniciar o serviço ao iniciar a aplicação
            await StartAsync();
            
            // Esperar até que o cancelamento seja solicitado
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            _logger.LogError(ex, "Erro no serviço de agendamento");
        }
        finally
        {
            // Parar o serviço ao encerrar a aplicação
            await StopAsync();
        }
    }

    // Implementações adicionais para suportar a API
    public async Task<ScheduledPostDto> SchedulePostAsync(CreateScheduledPostDto request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Criar uma entidade de domínio a partir do DTO
        var post = new ContentPost
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            Title = request.Title,
            Content = request.Content,
            Platform = request.Platform,
            ScheduledFor = request.ScheduledFor,
            Status = PostStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Salvar no repositório
        using var scope = _serviceProvider.CreateScope();
        var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();
        var savedPost = await contentPostRepository.AddAsync(post);

        // Mapear de volta para DTO
        return new ScheduledPostDto
        {
            Id = savedPost.Id,
            CreatorId = savedPost.CreatorId,
            Title = savedPost.Title,
            Content = savedPost.Content,
            ScheduledFor = savedPost.ScheduledFor ?? DateTime.UtcNow,
            Platform = savedPost.Platform,
            Status = savedPost.Status,
            MediaUrls = savedPost.MediaUrl != null ? new List<string> { savedPost.MediaUrl } : new List<string>(),
            Tags = new List<string>(),
            CreatedAt = savedPost.CreatedAt,
            UpdatedAt = savedPost.UpdatedAt
        };
    }

    public async Task<ScheduledPostDto> GetScheduledPostAsync(Guid id)
    {
        using var scope = _serviceProvider.CreateScope();
        var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();
        var post = await contentPostRepository.GetByIdAsync(id);

        if (post == null)
            return null;

        return new ScheduledPostDto
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            ScheduledFor = post.ScheduledFor ?? DateTime.UtcNow,
            Platform = post.Platform,
            Status = post.Status,
            MediaUrls = post.MediaUrl != null ? new List<string> { post.MediaUrl } : new List<string>(),
            Tags = new List<string>(),
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public async Task<ScheduledPostDto> UpdateScheduledPostAsync(Guid id, UpdateScheduledPostDto request)
    {
        using var scope = _serviceProvider.CreateScope();
        var contentPostRepository = scope.ServiceProvider.GetRequiredService<IContentPostRepository>();
        var post = await contentPostRepository.GetByIdAsync(id);

        if (post == null)
            return null;

        if (request.Title != null)
            post.Title = request.Title;
            
        if (request.Content != null)
            post.Content = request.Content;
            
        if (request.ScheduledFor.HasValue)
            post.ScheduledFor = request.ScheduledFor.Value;
            
        if (request.MediaUrls != null && request.MediaUrls.Count > 0)
            post.MediaUrl = request.MediaUrls[0];
            
        post.UpdatedAt = DateTime.UtcNow;
        
        var updatedPost = await contentPostRepository.UpdateAsync(post);
        
        return new ScheduledPostDto
        {
            Id = updatedPost.Id,
            CreatorId = updatedPost.CreatorId,
            Title = updatedPost.Title,
            Content = updatedPost.Content,
            ScheduledFor = updatedPost.ScheduledFor ?? DateTime.UtcNow,
            Platform = updatedPost.Platform,
            Status = updatedPost.Status,
            MediaUrls = updatedPost.MediaUrl != null ? new List<string> { updatedPost.MediaUrl } : new List<string>(),
            Tags = new List<string>(),
            CreatedAt = updatedPost.CreatedAt,
            UpdatedAt = updatedPost.UpdatedAt
        };
    }

    public async Task<List<ScheduledPostDto>> GetScheduledPostsInRangeDtoAsync(DateTime startDate, DateTime endDate)
    {
        var domainPosts = await ((ISchedulerService)this).GetScheduledPostsInRangeAsync(startDate, endDate);
        
        var dtoPosts = domainPosts.Select(post => new ScheduledPostDto
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            ScheduledFor = post.ScheduledFor ?? DateTime.UtcNow,
            Platform = post.Platform,
            Status = post.Status,
            MediaUrls = post.MediaUrl != null ? new List<string> { post.MediaUrl } : new List<string>(),
            Tags = new List<string>(),
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        }).ToList();
        
        return dtoPosts;
    }
} 