using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services
{
    /// <summary>
    /// Implementação do serviço avançado de análise de conteúdo
    /// </summary>
    public class ContentAnalysisService : IContentAnalysisService
    {
        private readonly IContentPostRepository _contentRepository;
        private readonly IContentCreatorRepository _creatorRepository;
        private readonly IContentPerformanceRepository _performanceRepository;
        private readonly ISocialMediaAccountRepository _accountRepository;
        private readonly IPerformanceMetricsRepository _metricsRepository;
        private readonly ILoggingService _loggingService;

        public ContentAnalysisService(
            IContentPostRepository contentRepository,
            IContentCreatorRepository creatorRepository,
            IContentPerformanceRepository performanceRepository,
            ISocialMediaAccountRepository accountRepository,
            IPerformanceMetricsRepository metricsRepository,
            ILoggingService loggingService)
        {
            _contentRepository = contentRepository;
            _creatorRepository = creatorRepository;
            _performanceRepository = performanceRepository;
            _accountRepository = accountRepository;
            _metricsRepository = metricsRepository;
            _loggingService = loggingService;
        }

        // Método implementado com algoritmos avançados
        public async Task<ContentInsightsDto> GetContentInsightsAsync(Guid contentId)
        {
            try
            {
                var contentPost = await _contentRepository.GetByIdAsync(contentId);
                if (contentPost == null)
                    throw new ArgumentException($"Conteúdo com ID {contentId} não encontrado");

                var performances = await _performanceRepository.GetByPostIdAsync(contentId.ToString());
                if (!performances.Any())
                    throw new InvalidOperationException($"Não há dados de performance para o conteúdo {contentId}");

                var creator = await _creatorRepository.GetByIdAsync(contentPost.CreatorId);
                if (creator == null)
                    throw new ArgumentException($"Criador com ID {contentPost.CreatorId} não encontrado");

                // Inicializa o objeto de insights
                var insights = new ContentInsightsDto
                {
                    ContentId = contentId,
                    Title = contentPost.Title,
                    Summary = contentPost.Content?.Substring(0, Math.Min(100, contentPost.Content?.Length ?? 0)) + "...",
                    PerformanceFactors = new Dictionary<string, double>(),
                    StrengthPoints = new List<string>(),
                    ImprovementSuggestions = new List<string>(),
                    PlatformPerformance = new Dictionary<SocialMediaPlatform, double>(),
                    ViralPotential = CalculateViralPotential(performances),
                    AudienceResponse = AnalyzeAudienceResponse(performances),
                    KeyAttributes = ExtractKeyAttributes(contentPost, performances),
                    CompetitorInsights = await AnalyzeCompetitionAsync(contentPost, creator)
                };

                // Análise de métricas de performance
                AnalyzePerformanceMetrics(performances, insights);

                // Identifica pontos fortes e sugestões de melhoria
                IdentifyStrengthsAndSuggestions(insights, contentPost.Platform);

                return insights;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Erro ao analisar conteúdo {contentId}: {ex.Message}");
                throw;
            }
        }

        // Método privado para análise de métricas de performance
        private void AnalyzePerformanceMetrics(IEnumerable<ContentPerformance> performances, ContentInsightsDto insights)
        {
            double totalViews = 0;
            double totalLikes = 0;
            double totalComments = 0;
            double totalShares = 0;
            double totalRevenue = 0;
            
            foreach (var perf in performances)
            {
                // Calcular taxa de engajamento para cada plataforma
                double engagementRate = CalculateEngagementRate(perf);
                insights.PlatformPerformance[perf.Platform] = engagementRate;
                
                // Acumular métricas
                totalViews += perf.Views;
                totalLikes += perf.Likes;
                totalComments += perf.Comments;
                totalShares += perf.Shares;
                totalRevenue += (double)perf.EstimatedRevenue;
                
                // Analisar fatores específicos de performance
                insights.PerformanceFactors[$"Likes ({perf.Platform})"] = perf.Views > 0 ? (double)perf.Likes / perf.Views * 100 : 0;
                insights.PerformanceFactors[$"Comentários ({perf.Platform})"] = perf.Views > 0 ? (double)perf.Comments / perf.Views * 100 : 0;
                insights.PerformanceFactors[$"Compartilhamentos ({perf.Platform})"] = perf.Views > 0 ? (double)perf.Shares / perf.Views * 100 : 0;
            }
            
            // Calcular scores globais
            int platformCount = performances.Select(p => p.Platform).Distinct().Count();
            if (platformCount > 0 && totalViews > 0)
            {
                insights.EngagementScore = (totalLikes + totalComments + totalShares) / totalViews;
                insights.ReachScore = totalViews / platformCount;
                insights.ConversionScore = totalRevenue > 0 ? totalRevenue / totalViews * 100 : 0;
                insights.RetentionScore = CalculateRetentionScore(performances);
                insights.SentimentScore = CalculateSentimentScore(performances);
                insights.OverallScore = (insights.EngagementScore * 0.3 + 
                                       insights.ReachScore * 0.2 + 
                                       insights.ConversionScore * 0.15 + 
                                       insights.RetentionScore * 0.15 + 
                                       insights.SentimentScore * 0.2) / 5;
            }
        }

        // Método para calcular potencial viral
        private ViralPotentialDto CalculateViralPotential(IEnumerable<ContentPerformance> performances)
        {
            var viralDto = new ViralPotentialDto
            {
                KeyFactors = new List<string>(),
                ShareProbabilities = new Dictionary<string, double>()
            };
            
            // Calcula taxa de compartilhamento
            double totalViews = performances.Sum(p => p.Views);
            double totalShares = performances.Sum(p => p.Shares);
            double shareRate = totalViews > 0 ? totalShares / totalViews * 100 : 0;
            
            // Calcula crescimento de visualizações nas primeiras 24-48 horas
            var orderedPerfs = performances.OrderBy(p => p.Date).ToList();
            double growthRate = CalculateGrowthRate(orderedPerfs);
            
            // Avalia velocidade de engajamento
            double engagementSpeed = CalculateEngagementSpeed(orderedPerfs);
            
            // Calcula score viral baseado em múltiplos fatores
            viralDto.Score = (shareRate * 0.5 + growthRate * 0.3 + engagementSpeed * 0.2);
            
            // Define avaliação baseada no score
            if (viralDto.Score > 8.0)
                viralDto.Assessment = "Potencial viral extremamente alto";
            else if (viralDto.Score > 6.0)
                viralDto.Assessment = "Alto potencial viral";
            else if (viralDto.Score > 4.0)
                viralDto.Assessment = "Potencial viral moderado";
            else if (viralDto.Score > 2.0)
                viralDto.Assessment = "Potencial viral baixo";
            else
                viralDto.Assessment = "Potencial viral muito baixo";
                
            // Identifica fatores-chave
            if (shareRate > 5.0)
                viralDto.KeyFactors.Add("Alta taxa de compartilhamento");
            if (growthRate > 50.0)
                viralDto.KeyFactors.Add("Rápido crescimento de visualizações");
            if (engagementSpeed > 7.0)
                viralDto.KeyFactors.Add("Engajamento rápido");
                
            // Estima probabilidades de compartilhamento por plataforma
            foreach (var group in performances.GroupBy(p => p.Platform))
            {
                var platformPerfs = group.ToList();
                double platformViews = platformPerfs.Sum(p => p.Views);
                double platformShares = platformPerfs.Sum(p => p.Shares);
                viralDto.ShareProbabilities[group.Key.ToString()] = platformViews > 0 ? platformShares / platformViews * 100 : 0;
            }
            
            return viralDto;
        }

        // Método para analisar resposta da audiência
        private AudienceResponseDto AnalyzeAudienceResponse(IEnumerable<ContentPerformance> performances)
        {
            var audienceResponse = new AudienceResponseDto
            {
                CommonFeedback = new List<string>(),
                DemographicBreakdown = new List<DemographicResponseDto>()
            };
            
            // Em uma implementação real, utilizaríamos análise de sentimento em comentários
            // e dados demográficos. Para esta implementação, geramos dados simulados.
            
            double likesRatio = performances.Sum(p => p.Likes) / (double)Math.Max(1, performances.Sum(p => p.Views));
            double commentsRatio = performances.Sum(p => p.Comments) / (double)Math.Max(1, performances.Sum(p => p.Views));
            
            // Proporção estimada de sentimentos com base em likes e comentários
            audienceResponse.PositiveSentiment = likesRatio * 0.8;
            audienceResponse.NegativeSentiment = (1 - likesRatio) * 0.3;
            audienceResponse.NeutralSentiment = 1 - (audienceResponse.PositiveSentiment + audienceResponse.NegativeSentiment);
            
            // Feedback comum simulado
            if (likesRatio > 0.1)
                audienceResponse.CommonFeedback.Add("Conteúdo bem recebido pela maioria da audiência");
            if (commentsRatio > 0.05)
                audienceResponse.CommonFeedback.Add("Gerou discussão significativa nos comentários");
            
            // Breakdown demográfico simulado
            audienceResponse.DemographicBreakdown.Add(new DemographicResponseDto
            {
                Demographic = "18-24 anos",
                EngagementRate = 0.12,
                ResponseType = "Muito Positivo"
            });
            
            audienceResponse.DemographicBreakdown.Add(new DemographicResponseDto
            {
                Demographic = "25-34 anos",
                EngagementRate = 0.08,
                ResponseType = "Positivo"
            });
            
            return audienceResponse;
        }

        // Método para extrair atributos-chave
        private List<string> ExtractKeyAttributes(ContentPost content, IEnumerable<ContentPerformance> performances)
        {
            var attributes = new List<string>();
            
            // Atributos baseados no tipo de conteúdo
            attributes.Add($"Tipo: {content.Platform}");
            
            // Atributos baseados na duração/tamanho
            if (content.Content.Length > 0)
                attributes.Add($"Tamanho do conteúdo: {content.Content.Length} caracteres");
            
            // Atributos baseados no tema/tópico (não existe na entidade, então ignoramos)
            // if (!string.IsNullOrEmpty(content.Topic))
            //    attributes.Add($"Tópico: {content.Topic}");
            
            // Atributos baseados na performance
            if (performances.Any())
            {
                double engagementRate = CalculateEngagementRate(performances.First());
                if (engagementRate > 0.1)
                    attributes.Add("Alto engajamento");
                else if (engagementRate < 0.01)
                    attributes.Add("Baixo engajamento");
            }
            
            return attributes;
        }

        // Método para analisar competição
        private async Task<List<CompetitorInsightDto>> AnalyzeCompetitionAsync(ContentPost content, ContentCreator creator)
        {
            var insights = new List<CompetitorInsightDto>();
            
            // Em uma implementação real, analisaríamos conteúdos similares de competidores
            // Para esta implementação, usamos dados simulados
            
            insights.Add(new CompetitorInsightDto
            {
                CompetitorName = "Competidor médio da categoria",
                RelativePerformance = 1.2, // 20% melhor que a média
                DifferentiatingFactors = new List<string>
                {
                    "Maior envolvimento da audiência",
                    "Melhor qualidade de produção"
                }
            });
            
            return insights;
        }

        // Método para identificar pontos fortes e sugestões
        private void IdentifyStrengthsAndSuggestions(ContentInsightsDto insights, SocialMediaPlatform platform)
        {
            // Identificar pontos fortes baseados em métricas
            if (insights.EngagementScore > 0.05)
                insights.StrengthPoints.Add("Alto nível de engajamento geral");
            
            if (insights.ReachScore > 1000)
                insights.StrengthPoints.Add("Boa performance de alcance");
            
            if (insights.SentimentScore > 0.7)
                insights.StrengthPoints.Add("Sentimento muito positivo da audiência");
            
            if (insights.ViralPotential.Score > 5.0)
                insights.StrengthPoints.Add("Bom potencial de viralização");
            
            // Identificar pontos fracos e sugestões
            if (insights.EngagementScore < 0.02)
                insights.ImprovementSuggestions.Add("Aumentar elementos interativos para melhorar engajamento");
            
            if (insights.ReachScore < 500)
                insights.ImprovementSuggestions.Add("Otimizar SEO e tags para aumentar alcance");
            
            if (insights.RetentionScore < 0.4)
                insights.ImprovementSuggestions.Add("Melhorar introdução e estrutura para aumentar retenção");
            
            // Sugestões específicas por tipo de plataforma
            switch (platform)
            {
                case SocialMediaPlatform.YouTube:
                    if (insights.RetentionScore < 0.5)
                        insights.ImprovementSuggestions.Add("Tornar os primeiros 15 segundos mais atrativos");
                    break;
                    
                case SocialMediaPlatform.Instagram:
                    if (insights.EngagementScore < 0.03)
                        insights.ImprovementSuggestions.Add("Usar legendas mais envolventes e call-to-actions");
                    break;
                    
                case SocialMediaPlatform.Twitter:
                    if (insights.RetentionScore < 0.3)
                        insights.ImprovementSuggestions.Add("Usar hashtags mais relevantes e conteúdo conciso");
                    break;
            }
        }

        // Métodos utilitários
        private double CalculateEngagementRate(ContentPerformance performance)
        {
            if (performance.Views <= 0)
                return 0;
                
            return (performance.Likes + performance.Comments + performance.Shares) / (double)performance.Views;
        }

        private double CalculateEngagementRate(IEnumerable<ContentPerformance> performances)
        {
            double totalEngagements = performances.Sum(p => p.Likes + p.Comments + p.Shares);
            double totalViews = performances.Sum(p => p.Views);
            
            return totalViews > 0 ? totalEngagements / totalViews : 0;
        }

        private double CalculateRetentionScore(IEnumerable<ContentPerformance> performances)
        {
            // Na implementação real, utilizaríamos métricas de retenção da plataforma
            // Para esta implementação, simulamos baseado no engajamento
            double engagementRate = CalculateEngagementRate(performances);
            return Math.Min(1.0, engagementRate * 10);
        }

        private double CalculateSentimentScore(IEnumerable<ContentPerformance> performances)
        {
            // Na implementação real, utilizaríamos análise de sentimento em comentários
            // Para esta implementação, simulamos baseado na proporção de likes
            double totalLikes = performances.Sum(p => p.Likes);
            double totalReactions = totalLikes + performances.Sum(p => p.Comments);
            
            return totalReactions > 0 ? totalLikes / totalReactions : 0.5;
        }

        private double CalculateGrowthRate(List<ContentPerformance> orderedPerformances)
        {
            // Implementação simplificada - em produção usaríamos dados temporais mais detalhados
            if (orderedPerformances.Count < 2)
                return 0;
                
            var first = orderedPerformances.First();
            var last = orderedPerformances.Last();
            
            TimeSpan timeSpan = last.Date - first.Date;
            if (timeSpan.TotalHours < 1)
                return 0;
                
            return (last.Views - first.Views) / timeSpan.TotalHours;
        }

        private double CalculateEngagementSpeed(List<ContentPerformance> orderedPerformances)
        {
            // Implementação simplificada - em produção analisaríamos a velocidade de engajamento por hora
            if (orderedPerformances.Count < 2)
                return 0;
                
            var first = orderedPerformances.First();
            var last = orderedPerformances.Last();
            
            TimeSpan timeSpan = last.Date - first.Date;
            if (timeSpan.TotalHours < 1)
                return 0;
                
            double totalFirstEngagements = first.Likes + first.Comments + first.Shares;
            double totalLastEngagements = last.Likes + last.Comments + last.Shares;
            
            return (totalLastEngagements - totalFirstEngagements) / timeSpan.TotalHours;
        }

        // Método implementado com algoritmos avançados
        public async Task<HighPerformancePatternDto> AnalyzeHighPerformancePatternsAsync(Guid creatorId, int topPostsCount = 20)
        {
            try
            {
                var creator = await _creatorRepository.GetByIdAsync(creatorId);
                if (creator == null)
                    throw new ArgumentException($"Criador com ID {creatorId} não encontrado");
            
                // Buscar posts do criador
                var posts = await _contentRepository.GetByCreatorIdAsync(creatorId);
                if (!posts.Any())
                    throw new InvalidOperationException($"Não há conteúdos para o criador {creatorId}");
            
                // Buscar dados de performance para cada post individualmente e consolidar
                var allPerformances = new List<ContentPerformance>();
                foreach (var post in posts)
                {
                    var postPerformances = await _performanceRepository.GetByPostIdAsync(post.Id.ToString());
                    if (postPerformances.Any())
                    {
                        allPerformances.AddRange(postPerformances);
                    }
                }
            
                // Ordenar posts por engajamento e pegar os top N
                var postsWithPerformance = posts
                    .Select(post => new 
                    {
                        Post = post,
                        Performances = allPerformances.Where(p => p.PostId.ToString() == post.Id.ToString()).ToList()
                    })
                    .Where(x => x.Performances.Any()) // Filtrar apenas posts com dados de performance
                    .OrderByDescending(x => CalculateEngagementScore(x.Performances))
                    .Take(topPostsCount)
                    .ToList();
            
                if (!postsWithPerformance.Any())
                    throw new InvalidOperationException($"Não há dados de performance suficientes para analisar padrões");
            
                // Converter para List<dynamic> para compatibilidade com os métodos de análise
                List<dynamic> dynamicPosts = postsWithPerformance.Cast<dynamic>().ToList();
            
                // Inicializar o objeto de resultado
                var result = new HighPerformancePatternDto
                {
                    CreatorId = creatorId,
                    IdentifiedPatterns = new List<ContentPatternDto>(),
                    TimingPatterns = AnalyzeTimingPatterns(dynamicPosts),
                    TopicPatterns = AnalyzeTopicPatterns(dynamicPosts),
                    FormatPatterns = AnalyzeFormatPatterns(dynamicPosts),
                    StylePatterns = AnalyzeStylePatterns(dynamicPosts),
                    AttributeCorrelations = CalculateAttributeCorrelations(dynamicPosts),
                    HighPerformingFormats = IdentifyHighPerformingFormats(dynamicPosts)
                };
            
                // Identificar padrões principais de conteúdo
                IdentifyMainContentPatterns(dynamicPosts, result);
            
                return result;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Erro ao analisar padrões de alto desempenho para {creatorId}: {ex.Message}");
                throw;
            }
        }

        // Método para analisar padrões de tempo
        private List<TimingPatternDto> AnalyzeTimingPatterns(List<dynamic> postsWithPerformance)
        {
            var result = new List<TimingPatternDto>();
            
            // Agrupar por dia da semana
            var dayGroups = postsWithPerformance
                .SelectMany(x => (IEnumerable<ContentPerformance>)x.Performances)
                .GroupBy(p => p.Date.DayOfWeek)
                .Select(g => new 
                {
                    Day = g.Key,
                    AvgEngagement = g.Average(p => CalculateEngagementRate(p)),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgEngagement)
                .ToList();
            
            // Agrupar por hora do dia
            var timeGroups = postsWithPerformance
                .SelectMany(x => (IEnumerable<ContentPerformance>)x.Performances)
                .GroupBy(p => p.Date.Hour)
                .Select(g => new 
                {
                    Hour = g.Key,
                    AvgEngagement = g.Average(p => CalculateEngagementRate(p)),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgEngagement)
                .ToList();
            
            // Agrupar por plataforma, dia e hora
            var platformGroups = postsWithPerformance
                .SelectMany(x => (IEnumerable<ContentPerformance>)x.Performances)
                .GroupBy(p => p.Platform)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(p => new { p.Date.DayOfWeek, Hour = p.Date.Hour })
                         .Select(dg => new 
                         {
                             Day = dg.Key.DayOfWeek,
                             Hour = dg.Key.Hour,
                             AvgEngagement = dg.Average(p => CalculateEngagementRate(p)),
                             Count = dg.Count()
                         })
                         .OrderByDescending(x => x.AvgEngagement)
                         .Take(3) // Top 3 slots de tempo por plataforma
                         .ToList()
                );
            
            // Criar padrão de timing
            var timingPattern = new TimingPatternDto
            {
                BestDays = dayGroups.Take(3).Select(x => x.Day).ToList(),
                BestTimes = timeGroups.Take(3).Select(x => new TimeSpan(x.Hour, 0, 0)).ToList(),
                PlatformSpecificTimes = new Dictionary<SocialMediaPlatform, List<BestTimeSlotDto>>(),
                ConfidenceScore = CalculateConfidenceScore(dayGroups.Sum(x => x.Count) + timeGroups.Sum(x => x.Count))
            };
            
            // Adicionar os melhores slots por plataforma
            foreach (var platform in platformGroups.Keys)
            {
                var bestSlots = platformGroups[platform].Select(x => new BestTimeSlotDto
                {
                    Day = x.Day,
                    Time = new TimeSpan(x.Hour, 0, 0),
                    EngagementScore = x.AvgEngagement,
                    Rationale = $"Baseado em {x.Count} posts com engajamento médio de {x.AvgEngagement:P2}"
                }).ToList();
                
                timingPattern.PlatformSpecificTimes[platform] = bestSlots;
            }
            
            result.Add(timingPattern);
            return result;
        }

        // Método para analisar padrões de tópico
        private List<TopicPatternDto> AnalyzeTopicPatterns(List<dynamic> postsWithPerformance)
        {
            var result = new List<TopicPatternDto>();
            
            // Em vez de Topic, usamos Title para agrupar posts semelhantes
            var topicGroups = postsWithPerformance
                .Where(x => !string.IsNullOrEmpty((string)x.Post.Title))
                .GroupBy(x => ExtractTopicFromTitle((string)x.Post.Title))
                .Select(g => new 
                {
                    Topic = g.Key,
                    Posts = g.ToList(),
                    AvgEngagement = g.Average(p => CalculateEngagementScore((List<ContentPerformance>)p.Performances)),
                    Count = g.Count()
                })
                .Where(x => x.Count >= 2) // Ao menos 2 posts por tópico para ser considerado
                .OrderByDescending(x => x.AvgEngagement)
                .Take(5) // Top 5 tópicos
                .ToList();
            
            foreach (var topicGroup in topicGroups)
            {
                // Calcular tendência de crescimento baseado nas datas dos posts
                double growthTrend = CalculateTopicGrowthTrend(topicGroup.Posts);
                
                // Identificar tópicos relacionados e convertê-los para strings
                var relatedTopics = IdentifyRelatedTopics(topicGroup.Topic, postsWithPerformance);
                List<string> stringRelatedTopics = relatedTopics.Select(x => x.ToString()).ToList();
                
                // Extrair palavras-chave do tópico e convertê-las para strings
                var keywords = ExtractKeywords(topicGroup.Posts);
                List<string> stringKeywords = keywords.Select(x => x.ToString()).ToList();
                
                result.Add(new TopicPatternDto
                {
                    TopicName = topicGroup.Topic,
                    EngagementScore = topicGroup.AvgEngagement,
                    GrowthTrend = growthTrend,
                    RelatedTopics = stringRelatedTopics,
                    Keywords = stringKeywords
                });
            }
            
            return result;
        }

        // Extrai um tópico do título do post
        private string ExtractTopicFromTitle(string title)
        {
            // Implementação simples - em produção usaríamos NLP
            // Pega a primeira parte do título até um separador comum
            var separators = new[] { "-", ":", "|", "—" };
            foreach (var sep in separators)
            {
                int index = title.IndexOf(sep);
                if (index > 0)
                    return title.Substring(0, index).Trim();
            }
            
            // Se não encontrar separador, retorna até 30 caracteres do título
            return title.Length <= 30 ? title : title.Substring(0, 30);
        }

        // Método para analisar padrões de formato
        private List<FormatPatternDto> AnalyzeFormatPatterns(List<dynamic> postsWithPerformance)
        {
            var result = new List<FormatPatternDto>();
            
            // Agrupar por plataforma como substituto para tipo de conteúdo
            var formatGroups = postsWithPerformance
                .GroupBy(x => (SocialMediaPlatform)x.Post.Platform)
                .Select(g => new 
                {
                    Format = g.Key,
                    Posts = g.ToList(),
                    Platforms = g.SelectMany(p => (IEnumerable<ContentPerformance>)p.Performances)
                                 .GroupBy(perf => perf.Platform)
                                 .OrderByDescending(pg => pg.Average(perf => CalculateEngagementRate(perf)))
                                 .Select(pg => pg.Key)
                                 .ToList(),
                    AvgEngagement = g.Average(p => CalculateEngagementScore((List<ContentPerformance>)p.Performances)),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgEngagement)
                .ToList();
            
            foreach (var formatGroup in formatGroups)
            {
                // Determinar tamanho ótimo para este formato
                string optimalSize = DetermineOptimalSize(formatGroup.Posts);
                
                // Identificar melhores práticas para este formato
                var bestPractices = IdentifyFormatBestPractices(formatGroup.Format, formatGroup.Posts);
                
                result.Add(new FormatPatternDto
                {
                    FormatName = GetFormatNameByPlatform(formatGroup.Format),
                    EngagementScore = formatGroup.AvgEngagement,
                    BestPlatforms = formatGroup.Platforms,
                    OptimalDuration = optimalSize,
                    BestPractices = bestPractices
                });
            }
            
            return result;
        }

        // Método que retorna o nome do formato baseado na plataforma
        private string GetFormatNameByPlatform(SocialMediaPlatform platform)
        {
            switch (platform)
            {
                case SocialMediaPlatform.YouTube: return "Vídeo";
                case SocialMediaPlatform.Instagram: return "Imagem/Carrossel";
                case SocialMediaPlatform.TikTok: return "Vídeo Curto";
                case SocialMediaPlatform.Twitter: return "Tweet";
                case SocialMediaPlatform.Facebook: return "Post";
                case SocialMediaPlatform.LinkedIn: return "Artigo";
                default: return platform.ToString();
            }
        }

        // Corrigindo o método IdentifyStylePatterns para evitar usar LINQ com objetos dinâmicos
        private List<StylePatternDto> AnalyzeStylePatterns(List<dynamic> postsWithPerformance)
        {
            var result = new List<StylePatternDto>();
            
            // Definir estilos a serem analisados
            var styles = new[] 
            {
                new { Name = "Storytelling", 
                      Predicate = new Func<dynamic, bool>(x => 
                                 ((string)x.Post.Content)?.Contains("história", StringComparison.OrdinalIgnoreCase) == true || 
                                 ((string)x.Post.Content)?.Contains("quando eu", StringComparison.OrdinalIgnoreCase) == true), 
                      Description = "Conteúdo narrativo com histórias pessoais ou de terceiros" },
                new { Name = "Inspiracional", 
                      Predicate = new Func<dynamic, bool>(x => 
                                 ((string)x.Post.Content)?.Contains("inspiração", StringComparison.OrdinalIgnoreCase) == true || 
                                 ((string)x.Post.Content)?.Contains("motivação", StringComparison.OrdinalIgnoreCase) == true), 
                      Description = "Conteúdo motivacional com foco em superação" },
                new { Name = "Direto ao Ponto", 
                      Predicate = new Func<dynamic, bool>(x => 
                                 ((string)x.Post.Content)?.Length < 500), 
                      Description = "Conteúdo conciso e objetivo com informações diretas" },
                new { Name = "Call-to-Action", 
                      Predicate = new Func<dynamic, bool>(x => 
                                 ((string)x.Post.Content)?.Contains("clique", StringComparison.OrdinalIgnoreCase) == true || 
                                 ((string)x.Post.Content)?.Contains("inscreva", StringComparison.OrdinalIgnoreCase) == true), 
                      Description = "Conteúdo com chamadas claras para ação" }
            };
            
            foreach (var style in styles)
            {
                // Selecionar posts que correspondem a este estilo
                var matchingPosts = new List<dynamic>();
                foreach (var post in postsWithPerformance)
                {
                    if (style.Predicate(post))
                    {
                        matchingPosts.Add(post);
                    }
                }
                
                if (matchingPosts.Count >= 2) // Precisamos de pelo menos 2 para considerar um padrão
                {
                    // Calcular média de engajamento usando nossa função auxiliar
                    double avgReception = CalculateAverageEngagement(matchingPosts);
                    
                    // Identificar características-chave deste estilo
                    var keyCharacteristics = IdentifyStyleCharacteristics(style.Name, matchingPosts);
                    
                    result.Add(new StylePatternDto
                    {
                        StyleName = style.Name,
                        Description = style.Description,
                        AudienceReception = avgReception,
                        KeyCharacteristics = keyCharacteristics
                    });
                }
            }
            
            return result;
        }

        // Método para calcular correlações entre atributos
        private Dictionary<string, double> CalculateAttributeCorrelations(List<dynamic> postsWithPerformance)
        {
            var correlations = new Dictionary<string, double>();
            
            // Correlacionar com comprimento do título
            correlations["Comprimento do Título"] = CorrelateWithEngagement(
                postsWithPerformance, 
                (dynamic x) => x.Post.Title?.Length ?? 0
            );
            
            // Correlacionar com comprimento do conteúdo
            correlations["Comprimento do Conteúdo"] = CorrelateWithEngagement(
                postsWithPerformance, 
                (dynamic x) => x.Post.Content?.Length ?? 0
            );
            
            // Correlacionar com URLs de mídia
            correlations["Presença de Mídia"] = CorrelateWithEngagement(
                postsWithPerformance, 
                (dynamic x) => string.IsNullOrEmpty(x.Post.MediaUrl) ? 0.0 : 1.0
            );
            
            // Correlacionar com horário de publicação (manhã/tarde/noite)
            correlations["Publicação Matinal"] = CorrelateWithEngagement(
                postsWithPerformance, 
                (dynamic x) => (x.Post.PublishedAt?.Hour >= 6 && x.Post.PublishedAt?.Hour < 12) ? 1.0 : 0.0
            );
            
            correlations["Publicação Vespertina"] = CorrelateWithEngagement(
                postsWithPerformance, 
                (dynamic x) => (x.Post.PublishedAt?.Hour >= 12 && x.Post.PublishedAt?.Hour < 18) ? 1.0 : 0.0
            );
            
            correlations["Publicação Noturna"] = CorrelateWithEngagement(
                postsWithPerformance, 
                (dynamic x) => (x.Post.PublishedAt?.Hour >= 18 || x.Post.PublishedAt?.Hour < 6) ? 1.0 : 0.0
            );
            
            return correlations;
        }

        // Método para identificar formatos de alto desempenho
        private Dictionary<string, double> IdentifyHighPerformingFormats(List<dynamic> postsWithPerformance)
        {
            var formats = new Dictionary<string, double>();
            
            // Agrupar por plataforma 
            var formatPlatformGroups = postsWithPerformance
                .SelectMany(x => ((IEnumerable<ContentPerformance>)x.Performances).Select(p => new 
                {
                    Platform = p.Platform,
                    Engagement = CalculateEngagementRate(p)
                }))
                .GroupBy(x => x.Platform)
                .Where(g => g.Count() >= 2) // Ao menos 2 exemplos para considerar
                .Select(g => new 
                {
                    Format = $"Conteúdo para {g.Key}",
                    AvgEngagement = g.Average(x => x.Engagement),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgEngagement)
                .Take(5) // Top 5 formatos
                .ToDictionary(x => x.Format, x => x.AvgEngagement);
            
            return formatPlatformGroups;
        }

        // Método para identificar padrões principais de conteúdo
        private void IdentifyMainContentPatterns(List<dynamic> postsWithPerformance, HighPerformancePatternDto result)
        {
            // Padrão 1: Conteúdo de alto engajamento
            var highEngagement = postsWithPerformance
                .Where(x => CalculateEngagementScore(x.Performances) > 0.05)
                .Take(5)
                .ToList();
            
            if (highEngagement.Count >= 3)
            {
                // Lista de strings predefinida para os atributos
                List<string> engagementAttributes = new List<string> { "Interativo", "Ressonante", "Impactante" };
                
                result.IdentifiedPatterns.Add(new ContentPatternDto
                {
                    PatternName = "Conteúdo de Alto Engajamento",
                    Description = "Publicações que obtêm alto nível de interação da audiência",
                    ConfidenceScore = 0.8,
                    AverageEngagement = CalculateAverageEngagement(highEngagement),
                    ExampleContentIds = highEngagement.Select(x => ((Guid)x.Post.Id).ToString()).ToList(),
                    Attributes = engagementAttributes
                });
            }
            
            // Padrão 2: Conteúdo educativo
            var educationalContent = postsWithPerformance
                .Where(x => ((string)x.Post.Title)?.Contains("Como", StringComparison.OrdinalIgnoreCase) == true ||
                           ((string)x.Post.Content)?.Contains("aprenda", StringComparison.OrdinalIgnoreCase) == true)
                .Take(5)
                .ToList();
            
            if (educationalContent.Count >= 3)
            {
                // Lista de strings predefinida para os atributos
                List<string> educationalAttributes = new List<string> { "Educativo", "Informativo", "Tutorial" };
                
                result.IdentifiedPatterns.Add(new ContentPatternDto
                {
                    PatternName = "Conteúdo Educativo",
                    Description = "Tutoriais ou guias passo-a-passo com explicações detalhadas",
                    ConfidenceScore = 0.75,
                    AverageEngagement = CalculateAverageEngagement(educationalContent),
                    ExampleContentIds = educationalContent.Select(x => ((Guid)x.Post.Id).ToString()).ToList(),
                    Attributes = educationalAttributes
                });
            }
        }

        // Método auxiliar para calcular média de engajamento para uma lista de posts
        private double CalculateAverageEngagement(List<dynamic> posts)
        {
            if (posts == null || posts.Count == 0)
                return 0;

            double totalEngagement = 0;
            foreach (var post in posts)
            {
                totalEngagement += CalculateEngagementScore(post.Performances);
            }
            
            return totalEngagement / posts.Count;
        }

        // Métodos auxiliares

        private double CalculateEngagementScore(List<ContentPerformance> performances)
        {
            if (performances == null || !performances.Any())
                return 0;

            double totalEngagement = 0;
            
            foreach (var perf in performances)
            {
                // Calcular taxa de engajamento normalizada para cada plataforma
                double engagementRate = CalculateEngagementRate(perf);
                totalEngagement += engagementRate;
            }
            
            return totalEngagement / performances.Count;
        }

        private double CalculateEngagementScore(dynamic performances)
        {
            if (performances == null)
                return 0;

            // Converter para lista para poder contar
            List<dynamic> perfList = new List<dynamic>();
            foreach (var perf in performances)
            {
                perfList.Add(perf);
            }
            
            if (perfList.Count == 0)
                return 0;

            double totalEngagement = 0;
            
            foreach (var perf in perfList)
            {
                // Calcular taxa de engajamento normalizada para cada plataforma
                // Assumindo que o objeto dinâmico tem as propriedades necessárias
                int likes = perf.Likes;
                int comments = perf.Comments;
                int shares = perf.Shares;
                long views = perf.Views;
                
                double engagement = (likes * 1.0 + comments * 2.0 + shares * 3.0) / Math.Max(1, views);
                totalEngagement += engagement;
            }
            
            return totalEngagement / perfList.Count;
        }

        private double CalculateConfidenceScore(int sampleSize)
        {
            // Implementação simplificada - quanto mais amostras, maior a confiança
            // Em um sistema real, usaríamos métodos estatísticos mais robustos
            if (sampleSize < 5)
                return 0.3;
            else if (sampleSize < 10)
                return 0.5;
            else if (sampleSize < 20)
                return 0.7;
            else if (sampleSize < 50)
                return 0.85;
            else
                return 0.95;
        }

        private double CalculateTopicGrowthTrend(List<dynamic> posts)
        {
            // Comparar engajamento dos posts mais recentes vs. mais antigos
            var orderedPosts = posts
                .OrderBy(x => x.Post.PublishedAt ?? DateTime.MinValue)
                .ToList();
            
            if (orderedPosts.Count < 3)
                return 0;
            
            var firstHalf = orderedPosts.Take(orderedPosts.Count / 2).ToList();
            var secondHalf = orderedPosts.Skip(orderedPosts.Count / 2).ToList();
            
            // Corrigido para usar a sobrecarga com dynamic diretamente
            double firstHalfEngagement = 0;
            foreach (var post in firstHalf)
            {
                firstHalfEngagement += CalculateEngagementScore(post.Performances);
            }
            firstHalfEngagement = firstHalfEngagement / Math.Max(1, firstHalf.Count);
            
            double secondHalfEngagement = 0;
            foreach (var post in secondHalf)
            {
                secondHalfEngagement += CalculateEngagementScore(post.Performances);
            }
            secondHalfEngagement = secondHalfEngagement / Math.Max(1, secondHalf.Count);
            
            return firstHalfEngagement > 0 
                ? (secondHalfEngagement - firstHalfEngagement) / firstHalfEngagement * 100 
                : 0;
        }

        private List<object> IdentifyRelatedTopics(string mainTopic, List<dynamic> allPosts)
        {
            // Implementação simplificada - em um sistema real, usaríamos NLP mais avançado
            return allPosts
                .Where(x => !string.IsNullOrEmpty((string)x.Post.Title) && 
                            (string)x.Post.Title != mainTopic &&
                            (((string)x.Post.Title).Contains(mainTopic, StringComparison.OrdinalIgnoreCase) || 
                             mainTopic.Contains((string)x.Post.Title, StringComparison.OrdinalIgnoreCase)))
                .Select(x => (object)ExtractTopicFromTitle((string)x.Post.Title))
                .Distinct()
                .Take(3)
                .ToList();
        }

        private List<object> ExtractKeywords(List<dynamic> posts)
        {
            // Implementação simplificada - em um sistema real, usaríamos processamento de linguagem natural
            var words = posts
                .SelectMany(x => ((string)x.Post.Title + " " + (string)x.Post.Content)
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 3))
                .GroupBy(w => w.ToLowerInvariant())
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => (object)g.Key)
                .ToList();
            
            return words;
        }

        private string DetermineOptimalSize(List<dynamic> posts)
        {
            // Implementação adaptada para conteúdo de texto
            var contentGroups = posts
                .Where(x => !string.IsNullOrEmpty((string)x.Post.Content))
                .GroupBy(x => 
                {
                    var chars = ((string)x.Post.Content).Length;
                    if (chars < 100) return "Muito curto (< 100 caracteres)";
                    else if (chars < 500) return "Curto (100-500 caracteres)";
                    else if (chars < 1000) return "Médio (500-1000 caracteres)";
                    else if (chars < 2000) return "Longo (1000-2000 caracteres)";
                    else return "Muito longo (> 2000 caracteres)";
                })
                .Select(g => new 
                {
                    Size = g.Key,
                    AvgEngagement = g.Average(p => CalculateEngagementScore((List<ContentPerformance>)p.Performances)),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgEngagement)
                .ThenByDescending(x => x.Count)
                .FirstOrDefault();
            
            return contentGroups?.Size ?? "Não determinado";
        }

        private List<string> IdentifyFormatBestPractices(SocialMediaPlatform platform, List<dynamic> posts)
        {
            var practices = new List<string>();
            
            switch (platform)
            {
                case SocialMediaPlatform.YouTube:
                    practices.Add("Vídeos com títulos claros e descritivos têm melhor performance");
                    practices.Add("Incluir palavras-chave relevantes na descrição aumenta o alcance");
                    break;
                    
                case SocialMediaPlatform.Instagram:
                    practices.Add("Posts com imagens de alta qualidade e legendas envolventes têm melhor desempenho");
                    practices.Add("Usar hashtags relevantes aumenta a descoberta do conteúdo");
                    break;
                    
                case SocialMediaPlatform.TikTok:
                    practices.Add("Vídeos curtos e dinâmicos têm maior engajamento");
                    practices.Add("Participar de tendências aumenta significativamente o alcance");
                    break;
                    
                case SocialMediaPlatform.Twitter:
                    practices.Add("Posts concisos com hashtags estratégicas recebem mais interações");
                    practices.Add("Tweets com perguntas ou enquetes geram mais engajamento");
                    break;
            }
            
            // Práticas gerais
            practices.Add("Publicar consistentemente mantém o engajamento da audiência");
            
            return practices;
        }

        private List<string> IdentifyStyleCharacteristics(string style, List<dynamic> posts)
        {
            var characteristics = new List<string>();
            
            switch (style)
            {
                case "Educativo":
                    characteristics.Add("Explicações passo-a-passo");
                    characteristics.Add("Títulos que prometem aprendizado");
                    characteristics.Add("Uso de exemplos práticos");
                    break;
                    
                case "Motivacional":
                    characteristics.Add("Linguagem inspiradora");
                    characteristics.Add("Histórias de superação ou sucesso");
                    characteristics.Add("Uso de frases impactantes");
                    break;
                    
                case "Engajador":
                    characteristics.Add("Perguntas diretas à audiência");
                    characteristics.Add("Tópicos de interesse comum ao nicho");
                    characteristics.Add("Solicitação de opiniões e experiências");
                    break;
                    
                case "Call to Action":
                    characteristics.Add("Comandos diretos e claros");
                    characteristics.Add("Indicação explícita do próximo passo");
                    characteristics.Add("Uso de senso de urgência");
                    break;
            }
            
            return characteristics;
        }

        private double CorrelateWithEngagement(List<dynamic> posts, Func<dynamic, double> attributeSelector)
        {
            // Implementação simplificada de correlação
            // Em um sistema real, usaríamos o coeficiente de Pearson ou outros métodos estatísticos
            
            // Coletar os valores de engajamento e atributos explicitamente para cada post
            List<double> engagements = new List<double>();
            List<double> attributes = new List<double>();
            
            foreach (var post in posts)
            {
                engagements.Add(CalculateEngagementScore(post.Performances));
                attributes.Add(attributeSelector(post));
            }
            
            if (engagements.Count < 3 || attributes.Count < 3)
                return 0;
            
            // Calcular médias
            double engAvg = 0;
            foreach (var eng in engagements)
            {
                engAvg += eng;
            }
            engAvg /= engagements.Count;
            
            double attrAvg = 0;
            foreach (var attr in attributes)
            {
                attrAvg += attr;
            }
            attrAvg /= attributes.Count;
            
            // Calcular correlação
            double numerator = 0;
            double engDenominator = 0;
            double attrDenominator = 0;
            
            for (int i = 0; i < engagements.Count; i++)
            {
                double engDiff = engagements[i] - engAvg;
                double attrDiff = attributes[i] - attrAvg;
                
                numerator += engDiff * attrDiff;
                engDenominator += engDiff * engDiff;
                attrDenominator += attrDiff * attrDiff;
            }
            
            if (engDenominator <= 0 || attrDenominator <= 0)
                return 0;
            
            return numerator / Math.Sqrt(engDenominator * attrDenominator);
        }

        // Método implementado com algoritmos avançados de recomendação baseados em histórico
        public async Task<ContentRecommendationsDto> GenerateContentRecommendationsAsync(Guid creatorId)
        {
            try
            {
                var creator = await _creatorRepository.GetByIdAsync(creatorId);
                if (creator == null)
                    throw new ArgumentException($"Criador com ID {creatorId} não encontrado");
                
                // Buscar dados históricos para análise
                var posts = await _contentRepository.GetByCreatorIdAsync(creatorId);
                if (!posts.Any())
                    throw new InvalidOperationException($"Não há conteúdos para o criador {creatorId}");
                
                // Buscar métricas de performance para todos os posts
                var allPerformances = new List<ContentPerformance>();
                foreach (var post in posts)
                {
                    var postPerformances = await _performanceRepository.GetByPostIdAsync(post.Id.ToString());
                    if (postPerformances.Any())
                    {
                        allPerformances.AddRange(postPerformances);
                    }
                }
                
                // Criar os posts com suas performances para análise
                var postsWithPerformance = posts
                    .Select(post => new 
                    {
                        Post = post,
                        Performances = allPerformances.Where(p => p.PostId.ToString() == post.Id.ToString()).ToList()
                    })
                    .Where(x => x.Performances.Any())
                    .ToList();
                
                if (!postsWithPerformance.Any())
                    throw new InvalidOperationException($"Não há dados de performance suficientes para gerar recomendações");
                
                var recommendations = new ContentRecommendationsDto();
                
                // Corrigido a conversão de postsWithPerformance para List<dynamic>
                dynamic dynamicPostsWithPerformance = postsWithPerformance.Cast<dynamic>().ToList();
                
                // Gerar recomendações de tópicos com base em histórico
                recommendations.ContentTopics = await GenerateTopicRecommendationsAsync(dynamicPostsWithPerformance);
                
                // Gerar recomendações de formatos com base no desempenho histórico
                recommendations.ContentFormats = await GenerateFormatRecommendationsAsync(dynamicPostsWithPerformance);
                
                // Gerar estratégias de conteúdo que funcionaram bem no passado
                recommendations.ContentStrategies = GenerateContentStrategies(dynamicPostsWithPerformance);
                
                // Gerar táticas de engajamento baseadas em histórico
                recommendations.EngagementTactics = GenerateEngagementTactics(dynamicPostsWithPerformance);
                
                // Gerar oportunidades de monetização baseadas no histórico
                recommendations.MonetizationOpportunities = GenerateMonetizationOpportunities(creator, dynamicPostsWithPerformance);
                
                return recommendations;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Erro ao gerar recomendações de conteúdo para {creatorId}: {ex.Message}");
                throw;
            }
        }

        // Método para gerar recomendações de tópicos baseado no histórico
        private async Task<List<TopicRecommendationDto>> GenerateTopicRecommendationsAsync(List<dynamic> postsWithPerformance)
        {
            var recommendations = new List<TopicRecommendationDto>();
            
            // Extrair tópicos dos títulos e avaliar performance
            // Corrigido o uso de LINQ com lambdas em objetos dinâmicos
            var topicPerformance = new List<dynamic>();
            var groupedByTopic = new Dictionary<string, List<dynamic>>();
            
            // Agrupar manualmente por tópico
            foreach (var post in postsWithPerformance)
            {
                string topic = ExtractTopicFromTitle(post.Post.Title);
                if (!groupedByTopic.ContainsKey(topic))
                {
                    groupedByTopic[topic] = new List<dynamic>();
                }
                groupedByTopic[topic].Add(post);
            }
            
            // Processar cada grupo e criar a estrutura de dados necessária
            foreach (var group in groupedByTopic)
            {
                if (group.Value.Count >= 2) // Pelo menos 2 posts sobre o tópico
                {
                    double avgEngagement = CalculateAverageEngagement(group.Value);
                    long totalViews = 0;
                    
                    // Calcular visualizações totais
                    foreach (var post in group.Value)
                    {
                        foreach (var perf in post.Performances)
                        {
                            totalViews += perf.Views;
                        }
                    }
                    
                    topicPerformance.Add(new
                    {
                        Topic = group.Key,
                        Posts = group.Value,
                        AvgEngagement = avgEngagement,
                        Views = totalViews,
                        Count = group.Value.Count
                    });
                }
            }
            
            // Ordenar por engajamento
            topicPerformance = topicPerformance.OrderByDescending(t => t.AvgEngagement).ToList();
            
            // Identificar tópicos de melhor desempenho
            for (int i = 0; i < Math.Min(3, topicPerformance.Count); i++)
            {
                var topic = topicPerformance[i];
                double relevanceScore = Math.Min(0.95, 0.5 + topic.AvgEngagement);
                string potentialReach = DeterminePotentialReach(topic.Views, topic.Count);
                
                recommendations.Add(new TopicRecommendationDto
                {
                    Topic = topic.Topic,
                    RelevanceScore = relevanceScore,
                    PotentialReach = potentialReach,
                    RecommendationReason = $"Tópico com alto engajamento médio de {topic.AvgEngagement:P2} baseado em {topic.Count} posts anteriores"
                });
            }
            
            // Buscar tendências externas (simulado)
            var trendingTopics = new Dictionary<string, double>
            {
                { "Inteligência Artificial", 0.85 },
                { "Sustentabilidade", 0.78 },
                { "Web3", 0.72 },
                { "Bem-estar digital", 0.68 }
            };
            
            // Adicionar tópicos em tendência que ainda não foram muito explorados
            int count = 0;
            foreach (var trend in trendingTopics)
            {
                if (count >= 2) break;
                
                bool alreadyExists = false;
                foreach (var rec in recommendations)
                {
                    if (rec.Topic.Contains(trend.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        alreadyExists = true;
                        break;
                    }
                }
                
                if (!alreadyExists)
                {
                    recommendations.Add(new TopicRecommendationDto
                    {
                        Topic = trend.Key,
                        RelevanceScore = trend.Value,
                        PotentialReach = "Alto (tendência crescente)",
                        RecommendationReason = "Tópico em alta tendência com potencial de alcance significativo"
                    });
                    count++;
                }
            }
            
            return recommendations;
        }

        // Método para gerar recomendações de formato baseado no histórico
        private async Task<List<FormatRecommendationDto>> GenerateFormatRecommendationsAsync(List<dynamic> postsWithPerformance)
        {
            var recommendations = new List<FormatRecommendationDto>();
            
            // Analisar desempenho por plataforma e formato
            var platformPerformance = postsWithPerformance
                .GroupBy(p => p.Post.Platform)
                .Select(g => new 
                {
                    Platform = g.Key,
                    Posts = g.ToList(),
                    AvgEngagement = g.Average(p => CalculateEngagementScore(p.Performances)),
                    Count = g.Count()
                })
                .Where(p => p.Count >= 2) // Pelo menos 2 posts por plataforma
                .OrderByDescending(p => p.AvgEngagement)
                .ToList();
            
            // Recomendar melhores formatos por plataforma
            foreach (var platform in platformPerformance.Take(3))
            {
                var format = DetermineFormatForPlatform(platform.Platform);
                var bestPractices = IdentifyBestPractices(platform.Posts);
                var idealLength = DetermineIdealLength(platform.Posts);
                
                recommendations.Add(new FormatRecommendationDto
                {
                    Format = format,
                    PerformanceScore = Math.Min(0.95, 0.5 + platform.AvgEngagement),
                    IdealLength = idealLength,
                    BestPractices = bestPractices
                });
            }
            
            return recommendations;
        }

        // Métodos auxiliares para as recomendações
        private string DeterminePotentialReach(long totalViews, int postCount)
        {
            var avgViews = totalViews / Math.Max(1, postCount);
            
            if (avgViews > 10000)
                return "Muito alto (10K+ visualizações)";
            else if (avgViews > 5000)
                return "Alto (5K-10K visualizações)";
            else if (avgViews > 1000)
                return "Médio (1K-5K visualizações)";
            else
                return "Moderado (menos de 1K visualizações)";
        }

        private string DetermineFormatForPlatform(SocialMediaPlatform platform)
        {
            switch (platform)
            {
                case SocialMediaPlatform.YouTube:
                    return "Vídeo longo (8-15 minutos)";
                case SocialMediaPlatform.TikTok:
                    return "Vídeo curto vertical (15-60 segundos)";
                case SocialMediaPlatform.Instagram:
                    return "Carrossel com 5-7 slides";
                case SocialMediaPlatform.Twitter:
                    return "Thread com 3-5 tweets";
                case SocialMediaPlatform.Facebook:
                    return "Post com texto e imagem";
                case SocialMediaPlatform.LinkedIn:
                    return "Artigo com 800-1200 palavras";
                default:
                    return "Formato misto (texto e mídia)";
            }
        }

        private List<string> IdentifyBestPractices(List<dynamic> posts)
        {
            var bestPractices = new List<string>();
            
            // Ordenar posts por engajamento
            var topPosts = posts
                .OrderByDescending(p => CalculateEngagementScore(p.Performances))
                .Take(3)
                .ToList();
            
            // Analisar características dos melhores posts
            bool includesMedia = topPosts.All(p => !string.IsNullOrEmpty(p.Post.MediaUrl));
            bool hasLongTitles = topPosts.Average(p => p.Post.Title?.Length ?? 0) > 50;
            bool hasCTA = topPosts.Any(p => p.Post.Content.Contains("comente", StringComparison.OrdinalIgnoreCase) || 
                                            p.Post.Content.Contains("compartilhe", StringComparison.OrdinalIgnoreCase));
            
            if (includesMedia)
                bestPractices.Add("Incluir elementos visuais ou mídia em todos os posts");
            
            if (hasLongTitles)
                bestPractices.Add("Usar títulos descritivos com mais de 50 caracteres");
            else
                bestPractices.Add("Manter títulos concisos com menos de 50 caracteres");
            
            if (hasCTA)
                bestPractices.Add("Incluir chamadas para ação claras no conteúdo");
            
            // Práticas gerais
            bestPractices.Add("Postar nos horários de pico de engajamento (18h-21h)");
            
            return bestPractices;
        }

        private string DetermineIdealLength(List<dynamic> posts)
        {
            // Agrupar posts por faixa de tamanho de conteúdo
            var lengthGroups = posts
                .GroupBy(p => 
                {
                    var contentLength = p.Post.Content?.Length ?? 0;
                    if (contentLength < 100) return "Muito curto";
                    else if (contentLength < 500) return "Curto";
                    else if (contentLength < 1000) return "Médio";
                    else if (contentLength < 2000) return "Longo";
                    else return "Muito longo";
                })
                .Select(g => new 
                {
                    LengthCategory = g.Key,
                    Posts = g.ToList(),
                    AvgEngagement = g.Average(p => CalculateEngagementScore(p.Performances)),
                    Count = g.Count()
                })
                .OrderByDescending(g => g.AvgEngagement)
                .FirstOrDefault();
            
            return lengthGroups?.LengthCategory ?? "Médio";
        }

        private List<string> GenerateContentStrategies(List<dynamic> postsWithPerformance)
        {
            var strategies = new List<string>();
            
            // Analisar padrões de publicação bem-sucedidos
            var topPosts = postsWithPerformance
                .OrderByDescending(p => CalculateEngagementScore(p.Performances))
                .Take(5)
                .ToList();
            
            // Verificar padrões de publicação
            bool weekendPerformsWell = topPosts.Count(p => 
                p.Post.CreatedAt.DayOfWeek == DayOfWeek.Saturday || 
                p.Post.CreatedAt.DayOfWeek == DayOfWeek.Sunday) > topPosts.Count / 2;
            
            if (weekendPerformsWell)
                strategies.Add("Priorizar publicações nos finais de semana quando o engajamento é maior");
            else
                strategies.Add("Focar publicações durante os dias de semana quando a audiência está mais ativa");
            
            // Analisar frequência de publicação
            var postsTimeline = postsWithPerformance
                .OrderBy(p => p.Post.CreatedAt)
                .Select(p => p.Post.CreatedAt)
                .ToList();
            
            if (postsTimeline.Count >= 2)
            {
                var avgDaysBetweenPosts = (postsTimeline.Last() - postsTimeline.First()).TotalDays / Math.Max(1, postsTimeline.Count - 1);
                
                if (avgDaysBetweenPosts < 2)
                    strategies.Add("Manter alta frequência de publicação diária para maximizar alcance");
                else if (avgDaysBetweenPosts < 7)
                    strategies.Add($"Manter periodicidade de publicação a cada {Math.Round(avgDaysBetweenPosts)} dias");
                else
                    strategies.Add("Aumentar frequência de publicação para pelo menos 2 vezes por semana");
            }
            
            // Estratégias gerais baseadas em análise do setor
            strategies.Add("Criar séries de conteúdo conectados para aumentar retenção da audiência");
            strategies.Add("Diversificar formatos para atingir diferentes segmentos da audiência");
            strategies.Add("Repurpose conteúdo de alto desempenho em múltiplas plataformas");
            
            return strategies;
        }

        private List<string> GenerateEngagementTactics(List<dynamic> postsWithPerformance)
        {
            var tactics = new List<string>();
            
            // Identificar fatores de engajamento em posts populares
            var highEngagementPosts = postsWithPerformance
                .OrderByDescending(p => CalculateEngagementScore(p.Performances))
                .Take(5)
                .ToList();
            
            // Verificar se perguntas melhoram engajamento
            bool questionsImproveEngagement = highEngagementPosts
                .Count(p => p.Post.Content.Contains("?")) > highEngagementPosts.Count / 2;
            
            if (questionsImproveEngagement)
                tactics.Add("Incluir perguntas diretas à audiência para estimular comentários");
            
            // Verificar eficácia de storytelling
            bool storytellingWorks = highEngagementPosts
                .Count(p => p.Post.Content.Length > 1000 && 
                           (p.Post.Content.Contains("quando eu", StringComparison.OrdinalIgnoreCase) || 
                            p.Post.Content.Contains("minha experiência", StringComparison.OrdinalIgnoreCase) ||
                            p.Post.Content.Contains("aconteceu", StringComparison.OrdinalIgnoreCase)))
                > highEngagementPosts.Count / 3;
            
            if (storytellingWorks)
                tactics.Add("Utilizar narrativas pessoais e storytelling para conectar com a audiência");
            
            // Táticas gerais baseadas em pesquisas do setor
            tactics.Add("Responder a todos os comentários nas primeiras 24 horas");
            tactics.Add("Utilizar calls-to-action específicos no final do conteúdo");
            tactics.Add("Criar conteúdo colaborativo com outros criadores para ampliar alcance");
            tactics.Add("Incentivar compartilhamentos oferecendo valor adicional");
            
            return tactics;
        }

        private List<string> GenerateMonetizationOpportunities(ContentCreator creator, List<dynamic> postsWithPerformance)
        {
            var opportunities = new List<string>();
            
            // Analisar engajamento médio
            double totalEngagement = 0;
            int postCount = 0;
            
            foreach (var post in postsWithPerformance)
            {
                totalEngagement += CalculateEngagementScore(post.Performances);
                postCount++;
            }
            
            double avgEngagement = postCount > 0 ? totalEngagement / postCount : 0;
            
            // Calcular visualizações totais
            double totalViews = 0;
            foreach (var post in postsWithPerformance)
            {
                foreach (var perf in post.Performances)
                {
                    totalViews += perf.Views;
                }
            }
            
            // Verificar potencial para produtos digitais
            if (avgEngagement > 0.05 && totalViews > 100000)
            {
                opportunities.Add("Desenvolver e-book ou curso digital sobre os tópicos mais engajados");
            }
            
            // Verificar potencial para parcerias
            if (creator.TotalFollowers > 10000 || totalViews > 200000)
            {
                opportunities.Add("Buscar parcerias com marcas para conteúdo patrocinado");
            }
            
            // Verificar oportunidades específicas por plataforma
            var platformsUsed = new HashSet<SocialMediaPlatform>();
            int youtubePostCount = 0;
            
            foreach (var post in postsWithPerformance)
            {
                foreach (var perf in post.Performances)
                {
                    platformsUsed.Add(perf.Platform);
                }
                
                if (post.Post.Platform == SocialMediaPlatform.YouTube)
                {
                    youtubePostCount++;
                }
            }
            
            if (platformsUsed.Contains(SocialMediaPlatform.YouTube) && youtubePostCount >= 10)
            {
                opportunities.Add("Otimizar monetização do YouTube com formato de vídeos acima de 8 minutos");
            }
            
            if (platformsUsed.Contains(SocialMediaPlatform.Instagram) && creator.TotalFollowers > 5000)
            {
                opportunities.Add("Explorar Instagram Shopping para produtos relacionados ao nicho");
            }
            
            // Oportunidades gerais
            opportunities.Add("Implementar links de afiliados em descrições de conteúdo relevante");
            opportunities.Add("Oferecer serviços de consultoria/mentoria no nicho de especialidade");
            opportunities.Add("Criar modelo de assinatura/comunidade para conteúdo premium");
            
            return opportunities;
        }

        public virtual async Task<AudienceInsightsDto> GetAudienceInsightsAsync(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            _loggingService.LogWarning($"Método GetAudienceInsightsAsync implementado parcialmente para {creatorId}");
            
            return new AudienceInsightsDto
            {
                CreatorId = creatorId,
                TotalAudienceSize = 0,
                GrowthRate = 0,
                DemographicBreakdown = new Dictionary<string, double>(),
                KeySegments = new List<AudienceSegmentDto>(),
                InterestDistribution = new Dictionary<string, double>(),
                PlatformEngagement = new Dictionary<SocialMediaPlatform, double>(),
                EngagementPatterns = new List<string>(),
                ContentPreferences = new Dictionary<string, double>(),
                LoyaltyMetrics = new LoyaltyMetricsDto()
            };
        }

        public virtual async Task<ContentComparisonDto> CompareContentTypesAsync(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            _loggingService.LogWarning($"Método CompareContentTypesAsync implementado parcialmente para {creatorId}");
            
            return new ContentComparisonDto
            {
                CreatorId = creatorId,
                StartDate = startDate,
                EndDate = endDate,
                ContentTypePerformance = new Dictionary<string, ContentTypePerformanceDto>(),
                PlatformSpecificPerformance = new Dictionary<SocialMediaPlatform, List<ContentTypePerformanceDto>>(),
                CrossPlatformInsights = new List<CrossPlatformInsightDto>(),
                AttributePerformance = new Dictionary<string, double>()
            };
        }

        public virtual async Task<ContentPredictionDto> PredictContentPerformanceAsync(ContentPredictionRequestDto request)
        {
            try
            {
                var creator = await _creatorRepository.GetByIdAsync(request.CreatorId);
                if (creator == null)
                    throw new ArgumentException($"Criador com ID {request.CreatorId} não encontrado");
                
                // Buscar histórico de posts e performance do criador
                var creatorPosts = await _contentRepository.GetByCreatorIdAsync(request.CreatorId);
                if (!creatorPosts.Any())
                    throw new InvalidOperationException($"Não há conteúdos históricos suficientes para predizer performance");
                
                // Buscar dados de performance para todos os posts
                var allPerformances = new List<ContentPerformance>();
                foreach (var post in creatorPosts)
                {
                    var postPerformances = await _performanceRepository.GetByPostIdAsync(post.Id.ToString());
                    if (postPerformances.Any())
                    {
                        allPerformances.AddRange(postPerformances);
                    }
                }
                
                // Construir dataset de treinamento com pares de post + performance
                var trainingData = creatorPosts
                    .Select(post => new 
                    {
                        Post = post,
                        Performances = allPerformances.Where(p => p.PostId.ToString() == post.Id.ToString()).ToList()
                    })
                    .Where(x => x.Performances.Any())
                    .ToList();
                    
                if (trainingData.Count < 5)
                    throw new InvalidOperationException("Dados históricos insuficientes para fazer previsões precisas");
                
                // Converter para dynamic para compatibilidade com o restante do código
                List<dynamic> dynamicTrainingData = trainingData.Cast<dynamic>().ToList();
                
                // Inicializar resultado da previsão
                var prediction = new ContentPredictionDto
                {
                    RequestId = Guid.NewGuid(),
                    Request = request,
                    MetricPredictions = new Dictionary<string, double>(),
                    FactorConfidenceScores = new Dictionary<string, double>(),
                    OptimizationSuggestions = new List<string>(),
                    PredictedAudience = new PredictedAudienceResponseDto
                    {
                        DemographicAppeal = new Dictionary<string, double>(),
                        SentimentDistribution = new Dictionary<string, double>(),
                        LikelyFeedback = new List<string>()
                    }
                };
                
                // Predizer engajamento base usando análise de posts similares
                var similarPosts = FindSimilarPosts(request, dynamicTrainingData);
                if (similarPosts.Any())
                {
                    // Calcular métricas básicas
                    prediction.PredictedEngagementScore = CalculatePredictedEngagementScore(request, similarPosts);
                    prediction.PredictedReachScore = CalculatePredictedReachScore(request, similarPosts);
                    prediction.PredictedViralPotential = CalculatePredictedViralPotential(request, similarPosts);
                    
                    // Prever métricas detalhadas
                    prediction.MetricPredictions = PredictDetailedMetrics(request, similarPosts);
                    
                    // Calcular scores de confiança dos fatores
                    prediction.FactorConfidenceScores = CalculateFactorConfidence(request, similarPosts);
                    
                    // Gerar sugestões de otimização
                    prediction.OptimizationSuggestions = GenerateOptimizationSuggestions(request, prediction);
                    
                    // Prever resposta da audiência
                    prediction.PredictedAudience = PredictAudienceResponse(request, similarPosts);
                    
                    // Calcular score geral de confiança
                    prediction.ConfidenceScore = CalculatePredictionConfidence(request, similarPosts);
                }
                else
                {
                    // Se não houver posts similares, usar médias gerais
                    prediction.PredictedEngagementScore = CalculateAverageEngagementFromTraining(dynamicTrainingData);
                    prediction.PredictedReachScore = CalculateAverageViews(dynamicTrainingData) * 0.8;
                    prediction.PredictedViralPotential = 0.3; // valor conservador
                    prediction.ConfidenceScore = 0.4; // confiança reduzida
                    
                    prediction.OptimizationSuggestions.Add("Adicionar mais elementos visuais para aumentar engajamento");
                    prediction.OptimizationSuggestions.Add("Utilizar hashtags mais relevantes para seu nicho");
                }
                
                return prediction;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Erro ao prever performance de conteúdo para {request.CreatorId}: {ex.Message}");
                throw;
            }
        }

        // Método para encontrar posts similares ao solicitado
        private List<dynamic> FindSimilarPosts(ContentPredictionRequestDto request, List<dynamic> trainingData)
        {
            // Implementação de similaridade usando múltiplos critérios ponderados
            // Versão sem LINQ para evitar problemas com dynamic
            var scoredPosts = new List<(dynamic Post, double SimilarityScore)>();
            
            foreach (var post in trainingData)
            {
                double similarityScore = CalculateSimilarityScore(request, post);
                scoredPosts.Add((post, similarityScore));
            }
            
            // Ordenar manualmente por similaridade
            scoredPosts.Sort((a, b) => b.SimilarityScore.CompareTo(a.SimilarityScore));
            
            // Filtrar e obter apenas os posts com similaridade acima do threshold
            var result = new List<dynamic>();
            foreach (var scoredPost in scoredPosts)
            {
                if (scoredPost.SimilarityScore > 0.4 && result.Count < 5)
                {
                    result.Add(scoredPost.Post);
                }
            }
            
            return result;
        }

        // Calcula a similaridade entre o conteúdo solicitado e um post histórico
        private double CalculateSimilarityScore(ContentPredictionRequestDto request, dynamic historicPost)
        {
            double score = 0.0;
            double totalWeight = 0.0;
            
            // Similaridade de plataforma (peso alto)
            double platformWeight = 0.3;
            if (request.TargetPlatform == historicPost.Post.Platform)
            {
                score += platformWeight;
            }
            totalWeight += platformWeight;
            
            // Similaridade de título (se houver)
            double titleWeight = 0.15;
            if (!string.IsNullOrEmpty(request.Title) && !string.IsNullOrEmpty(historicPost.Post.Title))
            {
                // Comparação simples por palavras-chave - correção para o uso de Intersect
                var requestWords = request.Title.ToLowerInvariant().Split(' ');
                string historicTitle = historicPost.Post.Title;
                var historicWords = historicTitle.ToLowerInvariant().Split(' ');
                
                int matchingWords = requestWords.Intersect(historicWords.Cast<string>()).Count();
                double titleSimilarity = (double)matchingWords / Math.Max(requestWords.Length, 1);
                
                score += titleWeight * titleSimilarity;
            }
            totalWeight += titleWeight;
            
            // Similaridade de conteúdo (se houver descrição)
            double contentWeight = 0.2;
            if (!string.IsNullOrEmpty(request.Description) && !string.IsNullOrEmpty(historicPost.Post.Content))
            {
                // Comparação simples por palavras-chave - correção para o uso de Intersect
                var requestWords = request.Description.ToLowerInvariant().Split(' ');
                string historicContent = historicPost.Post.Content;
                var historicWords = historicContent.ToLowerInvariant().Split(' ');
                
                int matchingWords = requestWords.Intersect(historicWords.Cast<string>()).Count();
                double contentSimilarity = (double)matchingWords / Math.Max(requestWords.Length, 1);
                
                score += contentWeight * contentSimilarity;
            }
            totalWeight += contentWeight;
            
            // Similaridade de tags (se houver)
            double tagsWeight = 0.15;
            if (request.Tags != null && request.Tags.Any())
            {
                // Assumindo que não temos tags no modelo histórico, então verificamos no conteúdo
                string historicTitle = historicPost.Post.Title;
                string historicContent = historicPost.Post.Content;
                
                int matchingTags = request.Tags.Count(tag => 
                    historicTitle.Contains(tag, StringComparison.OrdinalIgnoreCase) || 
                    historicContent.Contains(tag, StringComparison.OrdinalIgnoreCase));
                
                double tagSimilarity = (double)matchingTags / Math.Max(request.Tags.Count, 1);
                score += tagsWeight * tagSimilarity;
            }
            totalWeight += tagsWeight;
            
            // Normalizar score pelo peso total considerado
            return totalWeight > 0 ? score / totalWeight : 0;
        }

        // Calcula a pontuação de engajamento prevista com base em posts similares
        private double CalculatePredictedEngagementScore(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            // Média ponderada do engajamento dos posts similares
            double totalScore = 0;
            double totalWeight = 0;
            
            foreach (var post in similarPosts)
            {
                double similarity = CalculateSimilarityScore(request, post);
                double engagementScore = CalculateEngagementScore(post.Performances);
                
                totalScore += engagementScore * similarity;
                totalWeight += similarity;
            }
            
            // Aplicar ajustes baseados nos atributos específicos do pedido
            double baseScore = totalWeight > 0 ? totalScore / totalWeight : 0;
            
            // Ajuste para presença de call-to-action
            if (request.IncludesCallToAction)
            {
                baseScore *= 1.15; // +15% de engajamento
            }
            
            // Ajuste para horário de postagem
            double timeMultiplier = GetTimeOfDayMultiplier(request.PostTime);
            double dayMultiplier = GetDayOfWeekMultiplier(request.PostDay);
            
            return baseScore * timeMultiplier * dayMultiplier;
        }

        // Calcula a pontuação de alcance prevista
        private double CalculatePredictedReachScore(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            // Média de visualizações dos posts similares
            double totalViews = 0;
            foreach (var post in similarPosts)
            {
                double viewsSum = 0;
                foreach (var perf in post.Performances)
                {
                    viewsSum += perf.Views;
                }
                totalViews += viewsSum;
            }
            double avgViews = totalViews / similarPosts.Count;
            
            // Ajustes baseados na plataforma
            double platformMultiplier = GetPlatformReachMultiplier(request.TargetPlatform);
            
            // Ajuste para tipo de conteúdo
            double typeMultiplier = GetContentTypeReachMultiplier(request.Type);
            
            return avgViews * platformMultiplier * typeMultiplier;
        }

        // Calcula potencial viral previsto
        private double CalculatePredictedViralPotential(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            // Analisar indicadores de viralidade dos posts similares
            double totalShareRate = 0;
            int validPostCount = 0;
            
            foreach (var post in similarPosts)
            {
                double views = 0;
                double shares = 0;
                
                foreach (var perf in post.Performances)
                {
                    views += perf.Views;
                    shares += perf.Shares;
                }
                
                if (views > 0)
                {
                    totalShareRate += shares / views;
                    validPostCount++;
                }
            }
            
            double avgShareRate = validPostCount > 0 ? totalShareRate / validPostCount : 0;
            
            // Maior potencial para certos tipos de conteúdo
            double baseViralScore = avgShareRate * 10; // normalizar para escala 0-10
            
            // Ajustes por plataforma
            if (request.TargetPlatform == SocialMediaPlatform.TikTok)
                baseViralScore *= 1.3; // TikTok tem maior potencial viral
            else if (request.TargetPlatform == SocialMediaPlatform.Instagram)
                baseViralScore *= 1.2; // Instagram tem bom potencial viral
            
            // Ajustes por tipo de conteúdo
            if (request.Type == ContentType.Video)
                baseViralScore *= 1.25; // Vídeos têm maior potencial viral
            
            // Normalizar para escala 0-1
            return Math.Min(1.0, baseViralScore / 10.0);
        }

        // Método para prever métricas detalhadas
        private Dictionary<string, double> PredictDetailedMetrics(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            var metrics = new Dictionary<string, double>();
            
            // Calcular médias das métricas dos posts similares
            double totalViews = 0, totalLikes = 0, totalComments = 0, totalShares = 0, totalRevenue = 0;
            
            foreach (var post in similarPosts)
            {
                double postViews = 0, postLikes = 0, postComments = 0, postShares = 0, postRevenue = 0;
                
                foreach (var perf in post.Performances)
                {
                    postViews += perf.Views;
                    postLikes += perf.Likes;
                    postComments += perf.Comments;
                    postShares += perf.Shares;
                    postRevenue += (double)perf.EstimatedRevenue;
                }
                
                totalViews += postViews;
                totalLikes += postLikes;
                totalComments += postComments;
                totalShares += postShares;
                totalRevenue += postRevenue;
            }
            
            int postCount = similarPosts.Count;
            double avgViews = postCount > 0 ? totalViews / postCount : 0;
            double avgLikes = postCount > 0 ? totalLikes / postCount : 0;
            double avgComments = postCount > 0 ? totalComments / postCount : 0;
            double avgShares = postCount > 0 ? totalShares / postCount : 0;
            double avgRevenue = postCount > 0 ? totalRevenue / postCount : 0;
            
            // Aplicar multiplicadores específicos
            double platformMultiplier = GetPlatformReachMultiplier(request.TargetPlatform);
            double timeMultiplier = GetTimeOfDayMultiplier(request.PostTime) * GetDayOfWeekMultiplier(request.PostDay);
            
            // Adicionar previsões
            metrics["Visualizações"] = avgViews * platformMultiplier * timeMultiplier;
            metrics["Likes"] = avgLikes * platformMultiplier * timeMultiplier;
            metrics["Comentários"] = avgComments * platformMultiplier * timeMultiplier;
            metrics["Compartilhamentos"] = avgShares * platformMultiplier * timeMultiplier;
            metrics["Receita Estimada"] = avgRevenue * platformMultiplier * timeMultiplier;
            metrics["Taxa de Cliques"] = avgViews > 0 ? avgLikes / avgViews : 0;
            
            return metrics;
        }

        // Método para calcular confiança nos fatores
        private Dictionary<string, double> CalculateFactorConfidence(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            var confidenceScores = new Dictionary<string, double>();
            
            // Confiança baseada na quantidade de dados similares
            int postsWithMatchingPlatform = 0;
            foreach (var post in similarPosts)
            {
                if (post.Post.Platform.Equals(request.TargetPlatform))
                {
                    postsWithMatchingPlatform++;
                }
            }
            confidenceScores["Plataforma"] = (double)postsWithMatchingPlatform / Math.Max(1, similarPosts.Count);
            
            // Confiança no tipo de conteúdo
            confidenceScores["Tipo de Conteúdo"] = 0.7; // valor padrão, pois não temos ContentType no modelo atual
            
            // Confiança no horário
            int postsInSimilarTime = CountPostsWithSimilarTime(similarPosts, request.PostTime);
            confidenceScores["Horário"] = (double)postsInSimilarTime / Math.Max(1, similarPosts.Count);
            
            // Confiança no dia da semana
            int postsOnSameDay = CountPostsOnSameDay(similarPosts, request.PostDay);
            confidenceScores["Dia da Semana"] = (double)postsOnSameDay / Math.Max(1, similarPosts.Count);
            
            return confidenceScores;
        }

        // Método auxiliar usado no método CalculateFactorConfidence
        private int CountPostsWithSimilarTime(List<dynamic> similarPosts, TimeSpan requestTime, int hourThreshold = 2)
        {
            int count = 0;
            foreach (var post in similarPosts)
            {
                DateTime? publishedAt = post.Post.PublishedAt;
                if (publishedAt != null)
                {
                    TimeSpan postTime = publishedAt.Value.TimeOfDay;
                    if (Math.Abs((postTime - requestTime).TotalHours) < hourThreshold)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        // Método auxiliar usado no método CalculateFactorConfidence
        private int CountPostsOnSameDay(List<dynamic> similarPosts, DayOfWeek requestDay)
        {
            int count = 0;
            foreach (var post in similarPosts)
            {
                DateTime? publishedAt = post.Post.PublishedAt;
                if (publishedAt != null && publishedAt.Value.DayOfWeek == requestDay)
                {
                    count++;
                }
            }
            return count;
        }

        // Método para gerar sugestões de otimização
        private List<string> GenerateOptimizationSuggestions(ContentPredictionRequestDto request, ContentPredictionDto prediction)
        {
            var suggestions = new List<string>();
            
            // Sugestões baseadas nas métricas previstas
            if (prediction.PredictedEngagementScore < 0.03)
            {
                suggestions.Add("Adicione perguntas diretas para estimular comentários");
                suggestions.Add("Inclua um call-to-action claro e específico");
            }
            
            if (prediction.PredictedReachScore < 500)
            {
                suggestions.Add("Utilize hashtags mais populares relacionadas ao seu nicho");
                suggestions.Add("Publique em horários de maior atividade da sua audiência");
            }
            
            // Sugestões específicas por plataforma
            switch (request.TargetPlatform)
            {
                case SocialMediaPlatform.YouTube:
                    suggestions.Add("Otimize o título e descrição com palavras-chave relevantes");
                    suggestions.Add("Adicione miniaturas personalizadas com textos chamativos");
                    break;
                    
                case SocialMediaPlatform.Instagram:
                    suggestions.Add("Utilize entre 5-10 hashtags relevantes para maximizar descoberta");
                    suggestions.Add("Adicione um call-to-action na primeira parte da legenda");
                    break;
                    
                case SocialMediaPlatform.TikTok:
                    suggestions.Add("Mantenha o vídeo curto e dinâmico (15-30 segundos)");
                    suggestions.Add("Aproveite tendências e músicas populares");
                    break;
                    
                case SocialMediaPlatform.Twitter:
                    suggestions.Add("Inclua uma imagem ou GIF para aumentar engajamento");
                    suggestions.Add("Faça uma pergunta para estimular respostas");
                    break;
            }
            
            // Limitar para as 5 sugestões mais importantes
            return suggestions.Take(5).ToList();
        }

        // Método para prever resposta da audiência
        private PredictedAudienceResponseDto PredictAudienceResponse(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            var audienceResponse = new PredictedAudienceResponseDto
            {
                DemographicAppeal = new Dictionary<string, double>(),
                SentimentDistribution = new Dictionary<string, double>(),
                LikelyFeedback = new List<string>()
            };
            
            // Distribuição demográfica simulada
            audienceResponse.DemographicAppeal["18-24"] = 0.35;
            audienceResponse.DemographicAppeal["25-34"] = 0.40;
            audienceResponse.DemographicAppeal["35-44"] = 0.15;
            audienceResponse.DemographicAppeal["45+"] = 0.10;
            
            // Distribuição de sentimento baseada na taxa de likes
            double totalLikeRate = 0;
            int validPostCount = 0;
            
            foreach (var post in similarPosts)
            {
                double views = 0;
                double likes = 0;
                
                foreach (var perf in post.Performances)
                {
                    views += perf.Views;
                    likes += perf.Likes;
                }
                
                if (views > 0)
                {
                    totalLikeRate += likes / views;
                    validPostCount++;
                }
            }
            
            double avgLikeRate = validPostCount > 0 ? totalLikeRate / validPostCount : 0;
            
            audienceResponse.SentimentDistribution["Positivo"] = Math.Min(0.8, avgLikeRate * 2 + 0.5);
            audienceResponse.SentimentDistribution["Neutro"] = 0.9 - audienceResponse.SentimentDistribution["Positivo"];
            audienceResponse.SentimentDistribution["Negativo"] = 0.1;
            
            // Prováveis feedbacks baseados no tipo de conteúdo
            audienceResponse.LikelyFeedback.Add("Conteúdo relevante e útil");
            
            if (request.Type == ContentType.Video)
            {
                audienceResponse.LikelyFeedback.Add("Boa qualidade de produção");
                audienceResponse.LikelyFeedback.Add("Informações apresentadas de forma clara");
            }
            else
            {
                audienceResponse.LikelyFeedback.Add("Conteúdo bem escrito");
                audienceResponse.LikelyFeedback.Add("Informações valiosas para o nicho");
            }
            
            // Calcular retenção com base nos dados históricos
            audienceResponse.RetentionProbability = CalculatePredictedRetention(request, similarPosts);
            
            return audienceResponse;
        }

        // Método para calcular taxa de retenção prevista
        private double CalculatePredictedRetention(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            // Na falta de métricas reais de retenção, estimamos com base no engajamento
            double avgEngagement = similarPosts.Average(p => CalculateEngagementScore(p.Performances));
            
            // Ajustes baseados na duração/tamanho do conteúdo
            double retentionBase = Math.Min(0.9, 0.4 + avgEngagement * 2);
            
            if (request.Type == ContentType.Video && request.EstimatedDurationSeconds > 0)
            {
                // Vídeos mais longos tendem a ter menor retenção
                if (request.EstimatedDurationSeconds > 600) // > 10 minutos
                    retentionBase *= 0.8;
                else if (request.EstimatedDurationSeconds > 300) // > 5 minutos
                    retentionBase *= 0.9;
            }
            
            return retentionBase;
        }

        // Método para calcular confiança geral da previsão
        private double CalculatePredictionConfidence(ContentPredictionRequestDto request, List<dynamic> similarPosts)
        {
            // Base de confiança dependendo do número de posts similares
            double baseConfidence = Math.Min(0.9, 0.3 + (similarPosts.Count * 0.1));
            
            // Ajuste baseado na similaridade média
            double avgSimilarity = 0;
            foreach (var post in similarPosts)
            {
                avgSimilarity += CalculateSimilarityScore(request, post);
            }
            avgSimilarity = avgSimilarity / Math.Max(1, similarPosts.Count);
            
            // Ajuste baseado na consistência das métricas
            // Convertendo explicitamente para uma lista de double
            List<double> engagementScores = new List<double>();
            foreach (var post in similarPosts)
            {
                engagementScores.Add(CalculateEngagementScore(post.Performances));
            }
            
            double engagementStdDev = CalculateStandardDeviation(engagementScores);
            
            double consistencyFactor = 1.0 - Math.Min(0.5, engagementStdDev);
            
            return baseConfidence * avgSimilarity * consistencyFactor;
        }

        // Métodos auxiliares

        private double GetTimeOfDayMultiplier(TimeSpan postTime)
        {
            // Horários de pico aproximados
            if (postTime.Hours >= 17 && postTime.Hours <= 21) // Horário nobre
                return 1.2;
            else if (postTime.Hours >= 7 && postTime.Hours <= 9) // Manhã
                return 1.1;
            else if (postTime.Hours >= 12 && postTime.Hours <= 14) // Almoço
                return 1.15;
            else if (postTime.Hours >= 22 || postTime.Hours <= 5) // Madrugada
                return 0.8;
            else
                return 1.0; // Horário normal
        }

        private double GetDayOfWeekMultiplier(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return 1.15; // Fim de semana
                case DayOfWeek.Monday:
                case DayOfWeek.Friday:
                    return 1.1; // Início/fim da semana útil
                default:
                    return 1.0; // Dias úteis normais
            }
        }

        private double GetPlatformReachMultiplier(SocialMediaPlatform platform)
        {
            switch (platform)
            {
                case SocialMediaPlatform.YouTube:
                    return 1.2; // Maior alcance potencial
                case SocialMediaPlatform.TikTok:
                    return 1.3; // Algoritmo favorece conteúdo novo
                case SocialMediaPlatform.Instagram:
                    return 1.1;
                case SocialMediaPlatform.Twitter:
                    return 0.9; // Menor alcance orgânico
                default:
                    return 1.0;
            }
        }

        private double GetContentTypeReachMultiplier(ContentType type)
        {
            switch (type)
            {
                case ContentType.Video:
                    return 1.3; // Vídeos tendem a ter maior alcance
                case ContentType.SocialMedia:
                    return 1.1; // Corrigido de Image para SocialMedia
                case ContentType.Blog:
                    return 0.9; // Corrigido de Text para Blog
                default:
                    return 1.0;
            }
        }

        private double CalculateStandardDeviation(List<double> values)
        {
            if (values == null || values.Count <= 1)
                return 0;
            
            // Calcular média
            double sum = 0;
            foreach (var val in values)
            {
                sum += val;
            }
            double avg = sum / values.Count;
            
            // Calcular soma dos quadrados das diferenças
            double sumOfSquaresOfDifferences = 0;
            foreach (var val in values)
            {
                sumOfSquaresOfDifferences += Math.Pow(val - avg, 2);
            }
            
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / values.Count);
            
            return standardDeviation;
        }

        public async Task<AudienceSensitivityDto> AnalyzeAudienceSensitivityAsync(Guid creatorId)
        {
            _loggingService.LogWarning($"Método AnalyzeAudienceSensitivityAsync implementado parcialmente para {creatorId}");
            
            return new AudienceSensitivityDto
            {
                CreatorId = creatorId,
                ContentTypeSensitivity = new Dictionary<string, double>
                {
                    { "Vídeo", 0.8 },
                    { "Imagem", 0.7 },
                    { "Texto", 0.5 }
                },
                TopicSensitivity = new Dictionary<string, double>
                {
                    { "Tutorial", 0.9 },
                    { "Notícia", 0.6 },
                    { "Opinião", 0.5 }
                },
                StyleSensitivity = new Dictionary<string, double>
                {
                    { "Informal", 0.8 },
                    { "Técnico", 0.6 },
                    { "Humorístico", 0.7 }
                },
                TimingSensitivity = new Dictionary<string, double>
                {
                    { "Manhã", 0.7 },
                    { "Tarde", 0.8 },
                    { "Noite", 0.6 }
                },
                TopPreferences = new List<AudiencePreferenceDto>(),
                TopAversions = new List<AudienceAversion>(),
                OverallSensitivityScore = 0.7
            };
        }

        public virtual async Task<List<EngagementFactorDto>> IdentifyEngagementFactorsAsync(Guid creatorId)
        {
            try
            {
                var creator = await _creatorRepository.GetByIdAsync(creatorId);
                if (creator == null)
                    throw new ArgumentException($"Criador com ID {creatorId} não encontrado");
                
                // Buscar posts do criador
                var posts = await _contentRepository.GetByCreatorIdAsync(creatorId);
                if (!posts.Any())
                    throw new InvalidOperationException($"Não há conteúdos para o criador {creatorId}");
                
                // Buscar dados de performance para todos os posts
                var allPerformances = new List<ContentPerformance>();
                foreach (var post in posts)
                {
                    var postPerformances = await _performanceRepository.GetByPostIdAsync(post.Id.ToString());
                    if (postPerformances.Any())
                    {
                        allPerformances.AddRange(postPerformances);
                    }
                }
                
                // Criar os posts com suas performances para análise
                var postsWithPerformance = posts
                    .Select(post => new 
                    {
                        Post = post,
                        Performances = allPerformances.Where(p => p.PostId.ToString() == post.Id.ToString()).ToList()
                    })
                    .Where(x => x.Performances.Any())
                    .ToList();
                
                if (!postsWithPerformance.Any())
                    throw new InvalidOperationException($"Não há dados de performance suficientes para identificar fatores de engajamento");
                
                var engagementFactors = new List<EngagementFactorDto>();
                
                // Corrigido a conversão de postsWithPerformance para List<dynamic>
                dynamic dynamicPostsWithPerformance = postsWithPerformance.Cast<dynamic>().ToList();
                
                // Analisar fatores relacionados ao horário de publicação
                engagementFactors.Add(AnalyzeTimingFactor(dynamicPostsWithPerformance));
                
                // Analisar fatores relacionados ao tipo de conteúdo
                engagementFactors.Add(AnalyzeContentTypeFactor(dynamicPostsWithPerformance));
                
                // Analisar fatores relacionados à extensão do conteúdo
                engagementFactors.Add(AnalyzeContentLengthFactor(dynamicPostsWithPerformance));
                
                // Analisar fatores relacionados a elementos visuais
                engagementFactors.Add(AnalyzeVisualElementsFactor(dynamicPostsWithPerformance));
                
                // Analisar fatores relacionados às chamadas para ação
                engagementFactors.Add(AnalyzeCallToActionFactor(dynamicPostsWithPerformance));
                
                // Analisar fatores relacionados a hashtags
                engagementFactors.Add(AnalyzeHashtagsFactor(dynamicPostsWithPerformance));
                
                // Ordenar por importância
                return engagementFactors.OrderByDescending(f => f.Importance).ToList();
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Erro ao identificar fatores de engajamento para {creatorId}: {ex.Message}");
                throw;
            }
        }

        private EngagementFactorDto AnalyzeTimingFactor(List<dynamic> postsWithPerformance)
        {
            var factor = new EngagementFactorDto
            {
                FactorName = "Horário de Publicação",
                Description = "Impacto do horário e dia da semana no engajamento do conteúdo",
                SubFactors = new Dictionary<string, double>(),
                OptimizationTips = new List<string>(),
                ConfidenceScore = 0.8
            };
            
            // Analisar engajamento por dia da semana
            var dayPerformance = new Dictionary<DayOfWeek, List<double>>();
            foreach (var dayOfWeek in Enum.GetValues<DayOfWeek>())
            {
                dayPerformance[dayOfWeek] = new List<double>();
            }
            
            foreach (var post in postsWithPerformance)
            {
                var dayOfWeek = post.Post.PublishedAt?.DayOfWeek ?? DayOfWeek.Monday;
                var engagementScore = CalculateEngagementScore(post.Performances);
                dayPerformance[dayOfWeek].Add(engagementScore);
            }
            
            // Calcular média de engajamento por dia
            foreach (var day in dayPerformance.Keys.ToList())
            {
                if (dayPerformance[day].Any())
                {
                    factor.SubFactors[$"Dia: {day}"] = dayPerformance[day].Average();
                }
                else
                {
                    factor.SubFactors[$"Dia: {day}"] = 0;
                }
            }
            
            // Analisar engajamento por hora do dia
            var hourPerformance = new Dictionary<int, List<double>>();
            for (int i = 0; i < 24; i++)
            {
                hourPerformance[i] = new List<double>();
            }
            
            foreach (var post in postsWithPerformance)
            {
                var hour = post.Post.PublishedAt?.Hour ?? 12;
                var engagementScore = CalculateEngagementScore(post.Performances);
                hourPerformance[hour].Add(engagementScore);
            }
            
            // Calcular média de engajamento por hora
            var hourGroups = new Dictionary<string, double>
            {
                { "Manhã (6-11)", 0 },
                { "Tarde (12-17)", 0 },
                { "Noite (18-22)", 0 },
                { "Madrugada (23-5)", 0 }
            };
            
            int morningCount = 0, afternoonCount = 0, eveningCount = 0, nightCount = 0;
            
            for (int hour = 6; hour <= 11; hour++)
            {
                if (hourPerformance[hour].Any())
                {
                    hourGroups["Manhã (6-11)"] += hourPerformance[hour].Average();
                    morningCount++;
                }
            }
            
            for (int hour = 12; hour <= 17; hour++)
            {
                if (hourPerformance[hour].Any())
                {
                    hourGroups["Tarde (12-17)"] += hourPerformance[hour].Average();
                    afternoonCount++;
                }
            }
            
            for (int hour = 18; hour <= 22; hour++)
            {
                if (hourPerformance[hour].Any())
                {
                    hourGroups["Noite (18-22)"] += hourPerformance[hour].Average();
                    eveningCount++;
                }
            }
            
            for (int hour = 23; hour <= 23; hour++)
            {
                if (hourPerformance[hour].Any())
                {
                    hourGroups["Madrugada (23-5)"] += hourPerformance[hour].Average();
                    nightCount++;
                }
            }
            
            for (int hour = 0; hour <= 5; hour++)
            {
                if (hourPerformance[hour].Any())
                {
                    hourGroups["Madrugada (23-5)"] += hourPerformance[hour].Average();
                    nightCount++;
                }
            }
            
            // Normalizar médias por quantidade de horas
            if (morningCount > 0) hourGroups["Manhã (6-11)"] /= morningCount;
            if (afternoonCount > 0) hourGroups["Tarde (12-17)"] /= afternoonCount;
            if (eveningCount > 0) hourGroups["Noite (18-22)"] /= eveningCount;
            if (nightCount > 0) hourGroups["Madrugada (23-5)"] /= nightCount;
            
            // Adicionar períodos do dia como subfatores
            foreach (var period in hourGroups)
            {
                factor.SubFactors[period.Key] = period.Value;
            }
            
            // Encontrar os melhores horários e dias
            var bestDay = factor.SubFactors
                .Where(x => x.Key.StartsWith("Dia:"))
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();
            
            var bestPeriod = hourGroups
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();
            
            // Adicionar dicas de otimização
            factor.OptimizationTips.Add($"Priorize publicações em {bestDay.Key.Replace("Dia: ", "")} para maximizar engajamento");
            factor.OptimizationTips.Add($"O período de {bestPeriod.Key} apresenta maior engajamento da audiência");
            factor.OptimizationTips.Add("Mantenha consistência nos horários de publicação para construir expectativa na audiência");
            factor.OptimizationTips.Add("Teste diferentes horários periodicamente para acompanhar mudanças nos hábitos da audiência");
            
            // Calcular importância do fator
            var dayVariance = CalculateVariance(factor.SubFactors.Where(x => x.Key.StartsWith("Dia:")).Select(x => x.Value));
            var hourVariance = CalculateVariance(hourGroups.Values);
            
            factor.Importance = 0.5 + (Math.Min(dayVariance, 0.25) + Math.Min(hourVariance, 0.25));
            
            return factor;
        }

        private EngagementFactorDto AnalyzeContentTypeFactor(List<dynamic> postsWithPerformance)
        {
            var factor = new EngagementFactorDto
            {
                FactorName = "Tipo de Conteúdo",
                Description = "Impacto do formato e tipo de conteúdo no engajamento",
                SubFactors = new Dictionary<string, double>(),
                OptimizationTips = new List<string>(),
                ConfidenceScore = 0.75
            };
            
            // Agrupar por plataforma (como proxy para tipo de conteúdo)
            var platformGroups = postsWithPerformance
                .GroupBy(p => p.Post.Platform)
                .Select(g => new 
                {
                    Platform = g.Key,
                    Posts = g.ToList(),
                    AvgEngagement = g.Average(p => CalculateEngagementScore(p.Performances)),
                    Count = g.Count()
                })
                .Where(g => g.Count >= 2)
                .ToList();
            
            // Adicionar plataformas como subfatores
            foreach (var platform in platformGroups)
            {
                factor.SubFactors[$"Plataforma: {platform.Platform}"] = platform.AvgEngagement;
            }
            
            // Analisar presença de mídia
            var postsWithMedia = postsWithPerformance.Where(p => !string.IsNullOrEmpty(p.Post.MediaUrl)).ToList();
            var postsWithoutMedia = postsWithPerformance.Where(p => string.IsNullOrEmpty(p.Post.MediaUrl)).ToList();
            
            if (postsWithMedia.Any())
            {
                factor.SubFactors["Com Mídia"] = postsWithMedia.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            if (postsWithoutMedia.Any())
            {
                factor.SubFactors["Sem Mídia"] = postsWithoutMedia.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            // Encontrar os tipos de conteúdo mais eficazes
            var bestPlatform = platformGroups
                .OrderByDescending(p => p.AvgEngagement)
                .FirstOrDefault();
            
            bool mediaImproves = postsWithMedia.Any() && postsWithoutMedia.Any() && 
                                postsWithMedia.Average(p => CalculateEngagementScore(p.Performances)) > 
                                postsWithoutMedia.Average(p => CalculateEngagementScore(p.Performances));
            
            // Adicionar dicas de otimização
            if (bestPlatform != null)
            {
                factor.OptimizationTips.Add($"Priorize conteúdos para {bestPlatform.Platform} onde você obtém maior engajamento");
            }
            
            if (mediaImproves)
            {
                factor.OptimizationTips.Add("Inclua elementos de mídia em todos os posts para aumentar engajamento");
            }
            
            factor.OptimizationTips.Add("Diversifique formatos para alcançar diferentes segmentos da audiência");
            factor.OptimizationTips.Add("Adpate o conteúdo para as especificidades de cada plataforma");
            
            // Calcular importância do fator
            var platformVariance = CalculateVariance(platformGroups.Select(p => p.AvgEngagement));
            var mediaImpact = postsWithMedia.Any() && postsWithoutMedia.Any() ? 
                Math.Abs(factor.SubFactors["Com Mídia"] - factor.SubFactors["Sem Mídia"]) : 0;
            
            factor.Importance = 0.6 + (Math.Min(platformVariance, 0.2) + Math.Min(mediaImpact, 0.2));
            
            return factor;
        }

        private EngagementFactorDto AnalyzeContentLengthFactor(List<dynamic> postsWithPerformance)
        {
            var factor = new EngagementFactorDto
            {
                FactorName = "Extensão do Conteúdo",
                Description = "Impacto do tamanho e duração do conteúdo no engajamento",
                SubFactors = new Dictionary<string, double>(),
                OptimizationTips = new List<string>(),
                ConfidenceScore = 0.7
            };
            
            // Agrupar por tamanho de conteúdo
            var lengthGroups = postsWithPerformance
                .GroupBy(p => 
                {
                    var contentLength = p.Post.Content?.Length ?? 0;
                    if (contentLength < 100) return "Muito curto (<100 caracteres)";
                    else if (contentLength < 500) return "Curto (100-499 caracteres)";
                    else if (contentLength < 1000) return "Médio (500-999 caracteres)";
                    else if (contentLength < 2000) return "Longo (1000-1999 caracteres)";
                    else return "Muito longo (2000+ caracteres)";
                })
                .Select(g => new 
                {
                    LengthCategory = g.Key,
                    Posts = g.ToList(),
                    AvgEngagement = g.Average(p => CalculateEngagementScore(p.Performances)),
                    Count = g.Count()
                })
                .Where(g => g.Count >= 2)
                .ToList();
            
            // Adicionar categorias de tamanho como subfatores
            foreach (var group in lengthGroups)
            {
                factor.SubFactors[group.LengthCategory] = group.AvgEngagement;
            }
            
            // Encontrar o tamanho ideal
            var bestLengthGroup = lengthGroups
                .OrderByDescending(g => g.AvgEngagement)
                .FirstOrDefault();
            
            // Adicionar dicas de otimização
            if (bestLengthGroup != null)
            {
                factor.OptimizationTips.Add($"Conteúdos na categoria {bestLengthGroup.LengthCategory} têm melhor performance");
            }
            
            factor.OptimizationTips.Add("Ajuste o comprimento do conteúdo de acordo com a plataforma e contexto");
            factor.OptimizationTips.Add("Use parágrafos curtos e formatação para melhorar legibilidade");
            factor.OptimizationTips.Add("Considere a capacidade de atenção da audiência ao determinar o tamanho ideal");
            
            // Calcular importância do fator
            var lengthVariance = CalculateVariance(lengthGroups.Select(g => g.AvgEngagement));
            
            factor.Importance = 0.5 + Math.Min(lengthVariance, 0.3);
            
            return factor;
        }

        private EngagementFactorDto AnalyzeVisualElementsFactor(List<dynamic> postsWithPerformance)
        {
            var factor = new EngagementFactorDto
            {
                FactorName = "Elementos Visuais",
                Description = "Impacto de imagens, vídeos e outros elementos visuais no engajamento",
                SubFactors = new Dictionary<string, double>(),
                OptimizationTips = new List<string>(),
                ConfidenceScore = 0.75
            };
            
            // Verificar posts com diferentes tipos de mídia
            var postsWithImage = postsWithPerformance
                .Where(p => p.Post.MediaUrl != null && 
                           (p.Post.MediaUrl.EndsWith(".jpg") || 
                            p.Post.MediaUrl.EndsWith(".png") || 
                            p.Post.MediaUrl.EndsWith(".jpeg")))
                .ToList();
                
            var postsWithVideo = postsWithPerformance
                .Where(p => p.Post.MediaUrl != null && 
                           (p.Post.MediaUrl.EndsWith(".mp4") || 
                            p.Post.MediaUrl.EndsWith(".mov") || 
                            p.Post.MediaUrl.EndsWith(".avi")))
                .ToList();
            
            var postsWithoutVisuals = postsWithPerformance
                .Where(p => string.IsNullOrEmpty(p.Post.MediaUrl))
                .ToList();
            
            // Calcular engajamento médio para cada tipo
            if (postsWithImage.Any())
            {
                factor.SubFactors["Imagens"] = postsWithImage.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            if (postsWithVideo.Any())
            {
                factor.SubFactors["Vídeos"] = postsWithVideo.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            if (postsWithoutVisuals.Any())
            {
                factor.SubFactors["Sem elementos visuais"] = postsWithoutVisuals.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            // Determinar impacto dos elementos visuais
            bool imagesImproveEngagement = postsWithImage.Any() && postsWithoutVisuals.Any() &&
                                        factor.SubFactors["Imagens"] > factor.SubFactors["Sem elementos visuais"];
            
            bool videosImproveEngagement = postsWithVideo.Any() && postsWithoutVisuals.Any() &&
                                         factor.SubFactors["Vídeos"] > factor.SubFactors["Sem elementos visuais"];
            
            bool videosOutperformImages = postsWithVideo.Any() && postsWithImage.Any() &&
                                        factor.SubFactors["Vídeos"] > factor.SubFactors["Imagens"];
            
            // Adicionar dicas de otimização
            if (imagesImproveEngagement)
            {
                factor.OptimizationTips.Add("Inclua imagens de alta qualidade em seus posts para aumentar engajamento");
            }
            
            if (videosImproveEngagement)
            {
                factor.OptimizationTips.Add("Vídeos aumentam significativamente o engajamento da sua audiência");
            }
            
            if (videosOutperformImages)
            {
                factor.OptimizationTips.Add("Priorize conteúdo em vídeo sobre imagens estáticas quando possível");
            }
            
            factor.OptimizationTips.Add("Use elementos visuais que complementem e reforcem sua mensagem principal");
            factor.OptimizationTips.Add("Mantenha consistência visual para fortalecer reconhecimento da marca");
            
            // Calcular importância do fator
            double visualImpact = 0;
            int comparisons = 0;
            
            if (imagesImproveEngagement)
            {
                visualImpact += factor.SubFactors["Imagens"] - factor.SubFactors["Sem elementos visuais"];
                comparisons++;
            }
            
            if (videosImproveEngagement)
            {
                visualImpact += factor.SubFactors["Vídeos"] - factor.SubFactors["Sem elementos visuais"];
                comparisons++;
            }
            
            double avgVisualImpact = comparisons > 0 ? visualImpact / comparisons : 0;
            factor.Importance = 0.5 + Math.Min(avgVisualImpact, 0.4);
            
            return factor;
        }

        private EngagementFactorDto AnalyzeCallToActionFactor(List<dynamic> postsWithPerformance)
        {
            var factor = new EngagementFactorDto
            {
                FactorName = "Chamada para Ação (CTA)",
                Description = "Impacto de CTAs explícitos no engajamento da audiência",
                SubFactors = new Dictionary<string, double>(),
                OptimizationTips = new List<string>(),
                ConfidenceScore = 0.65
            };
            
            // Identificar posts com CTAs comuns
            var postsWithDirectCTA = postsWithPerformance
                .Where(p => 
                    p.Post.Content.Contains("comente", StringComparison.OrdinalIgnoreCase) ||
                    p.Post.Content.Contains("compartilhe", StringComparison.OrdinalIgnoreCase) ||
                    p.Post.Content.Contains("curta", StringComparison.OrdinalIgnoreCase) ||
                    p.Post.Content.Contains("inscreva", StringComparison.OrdinalIgnoreCase) ||
                    p.Post.Content.Contains("clique", StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            var postsWithQuestionCTA = postsWithPerformance
                .Where(p => p.Post.Content.Contains("?"))
                .ToList();
            
            var postsWithoutCTA = postsWithPerformance
                .Except(postsWithDirectCTA)
                .Except(postsWithQuestionCTA)
                .ToList();
            
            // Calcular engajamento médio para cada tipo
            if (postsWithDirectCTA.Any())
            {
                factor.SubFactors["CTA explícito"] = postsWithDirectCTA.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            if (postsWithQuestionCTA.Any())
            {
                factor.SubFactors["Perguntas"] = postsWithQuestionCTA.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            if (postsWithoutCTA.Any())
            {
                factor.SubFactors["Sem CTA"] = postsWithoutCTA.Average(p => CalculateEngagementScore(p.Performances));
            }
            
            // Determinar impacto dos CTAs
            bool directCTAWorks = postsWithDirectCTA.Any() && postsWithoutCTA.Any() &&
                                  factor.SubFactors["CTA explícito"] > factor.SubFactors["Sem CTA"];
            
            bool questionsWork = postsWithQuestionCTA.Any() && postsWithoutCTA.Any() &&
                                factor.SubFactors["Perguntas"] > factor.SubFactors["Sem CTA"];
            
            // Adicionar dicas de otimização
            if (directCTAWorks)
            {
                factor.OptimizationTips.Add("Inclua comandos explícitos para aumentar ações específicas da audiência");
            }
            
            if (questionsWork)
            {
                factor.OptimizationTips.Add("Faça perguntas diretas à audiência para estimular comentários");
            }
            
            factor.OptimizationTips.Add("Varie os tipos de CTA para evitar repetição e fadiga da audiência");
            factor.OptimizationTips.Add("Posicione o CTA principal no início ou fim do conteúdo para maior visibilidade");
            factor.OptimizationTips.Add("Crie senso de urgência nos CTAs quando apropriado");
            
            // Calcular importância do fator
            double ctaImpact = 0;
            int comparisons = 0;
            
            if (directCTAWorks)
            {
                ctaImpact += factor.SubFactors["CTA explícito"] - factor.SubFactors["Sem CTA"];
                comparisons++;
            }
            
            if (questionsWork)
            {
                ctaImpact += factor.SubFactors["Perguntas"] - factor.SubFactors["Sem CTA"];
                comparisons++;
            }
            
            double avgCtaImpact = comparisons > 0 ? ctaImpact / comparisons : 0;
            factor.Importance = 0.4 + Math.Min(avgCtaImpact, 0.4);
            
            return factor;
        }

        private EngagementFactorDto AnalyzeHashtagsFactor(List<dynamic> postsWithPerformance)
        {
            var factor = new EngagementFactorDto
            {
                FactorName = "Uso de Hashtags",
                Description = "Impacto da quantidade e relevância de hashtags no engajamento",
                SubFactors = new Dictionary<string, double>(),
                OptimizationTips = new List<string>(),
                ConfidenceScore = 0.6
            };
            
            // Função para contar hashtags no conteúdo
            Func<string, int> countHashtags = (content) =>
            {
                if (string.IsNullOrEmpty(content)) return 0;
                
                var matches = System.Text.RegularExpressions.Regex.Matches(content, @"#\w+");
                return matches.Count;
            };
            
            // Agrupar posts por quantidade de hashtags
            var hashtagGroups = postsWithPerformance
                .Select(p => new 
                {
                    Post = p,
                    HashtagCount = countHashtags(p.Post.Content),
                    EngagementScore = CalculateEngagementScore(p.Performances)
                })
                .GroupBy(p => 
                {
                    if (p.HashtagCount == 0) return "Sem hashtags";
                    else if (p.HashtagCount <= 3) return "1-3 hashtags";
                    else if (p.HashtagCount <= 7) return "4-7 hashtags";
                    else return "8+ hashtags";
                })
                .Select(g => new 
                {
                    Group = g.Key,
                    AvgEngagement = g.Average(p => p.EngagementScore),
                    Count = g.Count()
                })
                .Where(g => g.Count >= 2)
                .ToList();
            
            // Adicionar grupos como subfatores
            foreach (var group in hashtagGroups)
            {
                factor.SubFactors[group.Group] = group.AvgEngagement;
            }
            
            // Encontrar o grupo com melhor desempenho
            var bestGroup = hashtagGroups
                .OrderByDescending(g => g.AvgEngagement)
                .FirstOrDefault();
            
            // Adicionar dicas de otimização
            if (bestGroup != null)
            {
                factor.OptimizationTips.Add($"Utilize {bestGroup.Group} para maximizar engajamento");
            }
            
            factor.OptimizationTips.Add("Use hashtags específicas do seu nicho para alcançar audiência relevante");
            factor.OptimizationTips.Add("Inclua uma mistura de hashtags populares e específicas");
            factor.OptimizationTips.Add("Adapte a estratégia de hashtags para cada plataforma");
            factor.OptimizationTips.Add("Monitore e atualize regularmente as hashtags mais relevantes para seu nicho");
            
            // Calcular importância do fator
            var hashtagVariance = CalculateVariance(hashtagGroups.Select(g => g.AvgEngagement));
            
            factor.Importance = 0.3 + Math.Min(hashtagVariance, 0.3);
            
            return factor;
        }

        private double CalculateVariance(IEnumerable<double> values)
        {
            if (!values.Any()) return 0;
            
            double avg = values.Average();
            double sumOfSquaredDifferences = values.Sum(val => Math.Pow(val - avg, 2));
            return sumOfSquaredDifferences / values.Count();
        }

        // Corrigir o uso de LINQ Average em objetos dinâmicos
        private double CalculateAverageEngagementFromTraining(List<dynamic> trainingData)
        {
            double totalEngagement = 0;
            int count = 0;
            
            foreach (var data in trainingData)
            {
                totalEngagement += CalculateEngagementScore(data.Performances);
                count++;
            }
            
            return count > 0 ? totalEngagement / count : 0;
        }

        private double CalculateAverageViews(List<dynamic> trainingData)
        {
            double totalViews = 0;
            int count = 0;
            
            foreach (var data in trainingData)
            {
                double postViews = 0;
                foreach (var perf in data.Performances)
                {
                    postViews += perf.Views;
                }
                
                totalViews += postViews;
                count++;
            }
            
            return count > 0 ? totalViews / count : 0;
        }
    }
} 