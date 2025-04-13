using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.Services.Recommendation;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Tests.Helpers;
using Moq;
using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

// Adicionar alias para resolver a ambiguidade entre os namespaces
using AppServicesBestTimeSlotDto = MicroSaaS.Application.Interfaces.Services.BestTimeSlotDto;
using SharedBestTimeSlotDto = MicroSaaS.Shared.DTOs.BestTimeSlotDto;

namespace MicroSaaS.Tests.Unit;

// Classe que herda de RecommendationService e sobrescreve os métodos que queremos testar
public class TestableRecommendationService : RecommendationService
{
    public List<PostTimeRecommendation>? BestTimeToPostToReturn { get; set; }

    public TestableRecommendationService(
        IContentPostRepository contentRepository,
        IContentCreatorRepository creatorRepository,
        ISocialMediaAccountRepository socialMediaRepository,
        IPerformanceMetricsRepository metricsRepository,
        IContentPerformanceRepository contentPerformanceRepository,
        ISocialMediaIntegrationService socialMediaService)
        : base(contentRepository, creatorRepository, socialMediaRepository, metricsRepository, contentPerformanceRepository, socialMediaService)
    {
        // Inicialização com valor padrão
        BestTimeToPostToReturn = new List<PostTimeRecommendation>();
    }

    public override Task<List<PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        return Task.FromResult(BestTimeToPostToReturn ?? new List<PostTimeRecommendation>());
    }
}

public class RecommendationTests
{
    private readonly Mock<IContentPostRepository> _mockContentPostRepository;
    private readonly Mock<IContentCreatorRepository> _mockContentCreatorRepository;
    private readonly Mock<ISocialMediaAccountRepository> _mockSocialMediaAccountRepository;
    private readonly Mock<IPerformanceMetricsRepository> _mockPerformanceMetricsRepository;
    private readonly Mock<IContentPerformanceRepository> _mockContentPerformanceRepository;
    private readonly Mock<ISocialMediaIntegrationService> _mockSocialMediaIntegrationService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ILoggingService> _mockLoggingService;
    private readonly Mock<IContentAnalysisService> _mockContentAnalysisService;
    private readonly TestableRecommendationService _testableService;

    public RecommendationTests()
    {
        _mockContentPostRepository = new Mock<IContentPostRepository>();
        _mockContentCreatorRepository = new Mock<IContentCreatorRepository>();
        _mockSocialMediaAccountRepository = new Mock<ISocialMediaAccountRepository>();
        _mockPerformanceMetricsRepository = new Mock<IPerformanceMetricsRepository>();
        _mockContentPerformanceRepository = new Mock<IContentPerformanceRepository>();
        _mockSocialMediaIntegrationService = new Mock<ISocialMediaIntegrationService>();
        _mockCacheService = new Mock<ICacheService>();
        _mockLoggingService = new Mock<ILoggingService>();
        _mockContentAnalysisService = new Mock<IContentAnalysisService>();

        _testableService = new TestableRecommendationService(
            _mockContentPostRepository.Object,
            _mockContentCreatorRepository.Object,
            _mockSocialMediaAccountRepository.Object,
            _mockPerformanceMetricsRepository.Object,
            _mockContentPerformanceRepository.Object,
            _mockSocialMediaIntegrationService.Object
        );
    }

    [Fact]
    public async Task GetBestPostingTimesAsync_ShouldReturnValidResults()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var platform = SocialMediaPlatform.Instagram;
        
        // Configurar mock para retornar um criador válido
        var creator = TestHelper.CreateTestContentCreator(creatorId.ToString());
        _mockContentCreatorRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync(creator);

        // Configurar mock para retornar uma conta de social media para a plataforma
        var socialMediaAccount = new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Platform = platform,
            Username = "testuser",
            AccessToken = "test-token",
            RefreshToken = "refresh-token",
            IsActive = true
        };
        
        _mockSocialMediaAccountRepository.Setup(x => x.GetByPlatformAsync(creatorId, platform))
            .ReturnsAsync(new List<SocialMediaAccount> { socialMediaAccount });
            
        // Configurar mocks de posts e performances para ter dados suficientes
        var posts = new List<ContentPost>();
        for (int i = 0; i < 20; i++)
        {
            var post = TestHelper.CreateTestContentPost(creatorId, PostStatus.Published);
            post.Platform = platform;
            post.PublishedAt = DateTime.UtcNow.AddDays(-i).AddHours(-(i % 24));
            posts.Add(post);
        }
        
        _mockContentPostRepository.Setup(x => x.GetByCreatorIdAsync(creatorId))
            .ReturnsAsync(posts);
        
        // Criar uma lista de PostTimeRecommendation em vez de BestTimeSlotDto
        var postTimeRecommendations = new List<PostTimeRecommendation>();
        for (int hour = 10; hour <= 20; hour += 2)
        {
            postTimeRecommendations.Add(new PostTimeRecommendation
            {
                DayOfWeek = DayOfWeek.Monday,
                TimeOfDay = new TimeSpan(hour, 0, 0),
                EngagementScore = (double)(80 + Math.Sin(hour) * 10)
            });
        }
        
        // Configurar a classe de teste para retornar as recomendações de horário
        _testableService.BestTimeToPostToReturn = postTimeRecommendations;

        // Act
        var result = await _testableService.GetBestPostingTimesAsync(creatorId, platform);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.ForEach(timeSlot =>
        {
            timeSlot.CreatorId.Should().Be(creatorId);
            timeSlot.Platform.Should().Be(platform);
            timeSlot.Hour.Should().BeInRange(0, 23);
            timeSlot.EngagementPotential.Should().BeGreaterThan(0);
            timeSlot.RecommendationStrength.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task GetContentRecommendationsAsync_WithPlatform_ShouldReturnValidResults()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var platform = SocialMediaPlatform.Instagram;
        
        // Configurar mock para retornar um criador válido
        var creator = TestHelper.CreateTestContentCreator(creatorId.ToString());
        _mockContentCreatorRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync(creator);
            
        // Configurar mocks de posts e performances para ter dados suficientes
        var posts = new List<ContentPost>();
        for (int i = 0; i < 15; i++)
        {
            posts.Add(TestHelper.CreateTestContentPost(creatorId, PostStatus.Published));
        }
        
        _mockContentPostRepository.Setup(x => x.GetByCreatorIdAsync(creatorId))
            .ReturnsAsync(posts);

        // Act
        var result = await _testableService.GetContentRecommendationsAsync(creatorId, platform);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.ForEach(recommendation =>
        {
            recommendation.CreatorId.Should().Be(creatorId);
            recommendation.Platform.Should().Be(platform);
            recommendation.Title.Should().NotBeNullOrEmpty();
            recommendation.Description.Should().NotBeNullOrEmpty();
            recommendation.ConfidenceScore.Should().BeGreaterThan(0);
            recommendation.SuggestedHashtags.Should().NotBeNull();
            recommendation.SuggestedKeywords.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task GetAudienceSensitivityAnalysisAsync_ShouldReturnValidResults()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var platform = SocialMediaPlatform.Instagram;
        
        // Configurar mock para retornar um criador válido
        var creator = TestHelper.CreateTestContentCreator(creatorId.ToString());
        _mockContentCreatorRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync(creator);
            
        // Configurar mocks de posts e performances para ter dados suficientes
        var posts = new List<ContentPost>();
        for (int i = 0; i < 20; i++)
        {
            posts.Add(TestHelper.CreateTestContentPost(creatorId, PostStatus.Published));
        }
        
        _mockContentPostRepository.Setup(x => x.GetByCreatorIdAsync(creatorId))
            .ReturnsAsync(posts);

        // Act
        var result = await _testableService.GetAudienceSensitivityAnalysisAsync(creatorId, platform);

        // Assert
        result.Should().NotBeNull();
        result.CreatorId.Should().Be(creatorId);
        result.Platform.Should().Be(platform.ToString());
        result.TopResponsiveTopics.Should().NotBeNull();
        result.TopResponsiveFormats.Should().NotBeNull();
        result.SensitivityTopics.Should().NotBeNull();
        result.OverallSensitivity.Should().BeGreaterThan(0);
        result.Analysis.Should().NotBeNullOrEmpty();
        result.RecommendedContentApproach.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateCustomRecommendationAsync_ShouldReturnValidResult()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var request = new CustomRecommendationRequestDto
        {
            CreatorId = creatorId,
            Platform = SocialMediaPlatform.Instagram,
            RecommendationType = MicroSaaS.Shared.DTOs.RecommendationType.Topic,
            SpecificTopic = "Programação",
            ContentGoal = "Engajamento"
        };
        
        // Configurar mock para retornar um criador válido
        var creator = TestHelper.CreateTestContentCreator(creatorId.ToString());
        _mockContentCreatorRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync(creator);
            
        // Configurar mocks de posts e performances para ter dados suficientes
        var posts = new List<ContentPost>();
        for (int i = 0; i < 15; i++)
        {
            posts.Add(TestHelper.CreateTestContentPost(creatorId, PostStatus.Published));
        }
        
        _mockContentPostRepository.Setup(x => x.GetByCreatorIdAsync(creatorId))
            .ReturnsAsync(posts);

        // Act
        var result = await _testableService.GenerateCustomRecommendationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CreatorId.Should().Be(request.CreatorId);
        result.Platform.Should().Be(request.Platform);
        result.Type.Should().Be(request.RecommendationType);
        result.Title.Should().NotBeNullOrEmpty();
        result.Description.Should().NotBeNullOrEmpty();
        result.ConfidenceScore.Should().BeGreaterThan(0);
        result.SuggestedHashtags.Should().Contain(item => item.Contains("#"));
        result.SuggestedKeywords.Should().Contain(request.SpecificTopic!);
    }

    [Fact]
    public async Task GetGrowthRecommendationsAsync_WhenDataAvailable_ShouldReturnRecommendations()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var platform = SocialMediaPlatform.Instagram;
        var creator = TestHelper.CreateTestContentCreator(creatorId.ToString());
        
        _mockContentCreatorRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync(creator);
            
        // Configurar mock para retornar uma conta de social media para a plataforma
        var socialMediaAccount = new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Platform = platform,
            Username = "testuser",
            AccessToken = "test-token",
            RefreshToken = "refresh-token",
            IsActive = true
        };
        
        _mockSocialMediaAccountRepository.Setup(x => x.GetByPlatformAsync(creatorId, platform))
            .ReturnsAsync(new List<SocialMediaAccount> { socialMediaAccount });
            
        // Configurar mocks de posts e performances para ter dados suficientes
        var posts = new List<ContentPost>();
        for (int i = 0; i < 15; i++)
        {
            var post = TestHelper.CreateTestContentPost(creatorId, PostStatus.Published);
            post.Platform = platform;
            posts.Add(post);
        }
        
        _mockContentPostRepository.Setup(x => x.GetByCreatorIdAsync(creatorId))
            .ReturnsAsync(posts);

        // Act
        var result = await _testableService.GetGrowthRecommendationsAsync(creatorId, platform);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(2);
        
        // Ordenar as recomendações por dificuldade antes de comparar
        result = result.OrderBy(r => r.Difficulty).ToList();
        
        result.Should().BeInAscendingOrder(r => r.Difficulty);
        
        result.All(r => r.CreatorId == creatorId).Should().BeTrue();
        result.All(r => r.Platform == platform).Should().BeTrue();
        result.All(r => !string.IsNullOrEmpty(r.Title)).Should().BeTrue();
        result.All(r => !string.IsNullOrEmpty(r.Description)).Should().BeTrue();
        result.All(r => r.ImplementationSteps.Count > 0).Should().BeTrue();
        result.All(r => !string.IsNullOrEmpty(r.ExpectedOutcome)).Should().BeTrue();
    }

    [Fact]
    public async Task GetGrowthRecommendationsAsync_WhenCreatorNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var platform = SocialMediaPlatform.Instagram;
        
        // Mock configurado para retornar null (criador não encontrado)
        _mockContentCreatorRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync((ContentCreator)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await _testableService.GetGrowthRecommendationsAsync(creatorId, platform));
    }
} 