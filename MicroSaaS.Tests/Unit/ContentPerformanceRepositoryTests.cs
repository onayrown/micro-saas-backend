using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Infrastructure.Persistence.Repositories;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Tests.Helpers;
using MongoDB.Driver;
using Moq;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;
using System;
using MongoDB.Driver.Linq;
using System.Threading;

namespace MicroSaaS.Tests.Unit;

public class ContentPerformanceRepositoryTests
{
    private readonly Mock<IMongoCollection<ContentPerformance>> _collectionMock;
    private readonly Mock<IMongoDbContext> _contextMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly ContentPerformanceRepository _repository;
    private List<ContentPerformance> _performances;

    public ContentPerformanceRepositoryTests()
    {
        _collectionMock = new Mock<IMongoCollection<ContentPerformance>>();
        _contextMock = new Mock<IMongoDbContext>();
        _cacheMock = new Mock<ICacheService>();
        
        _contextMock.Setup(x => x.GetCollection<ContentPerformance>("ContentPerformances"))
            .Returns(_collectionMock.Object);

        _repository = new ContentPerformanceRepository(_contextMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPerformanceExists_ShouldReturnPerformance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new ContentPerformance
        {
            Id = id,
            CreatorId = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Platform = SocialMediaPlatform.Instagram,
            Views = 1000,
            Likes = 500,
            Comments = 100,
            Shares = 50,
            EngagementRate = 45.50m,
            Date = DateTime.UtcNow.AddDays(-7)
        };

        var cacheKey = $"perf:id:{id}";
        _cacheMock.Setup(x => x.GetAsync<ContentPerformance>(cacheKey))
            .ReturnsAsync((ContentPerformance)null);
            
        var mockCursor = new Mock<IAsyncCursor<ContentPerformance>>();
        mockCursor.Setup(x => x.Current).Returns(new List<ContentPerformance> { entity });
        mockCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<ContentPerformance>>(),
                It.IsAny<FindOptions<ContentPerformance, ContentPerformance>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCacheHit_ShouldReturnFromCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cachedEntity = new ContentPerformance
        {
            Id = id,
            CreatorId = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Platform = SocialMediaPlatform.Instagram,
            Views = 1500,
            Likes = 750,
            Comments = 200,
            Shares = 90,
            EngagementRate = 55.5m,
            Date = DateTime.UtcNow.AddDays(-3)
        };

        var cacheKey = $"perf:id:{id}";
        _cacheMock.Setup(x => x.GetAsync<ContentPerformance>(cacheKey))
            .ReturnsAsync(cachedEntity);

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(cachedEntity);
        _cacheMock.Verify(x => x.GetAsync<ContentPerformance>(cacheKey), Times.Once);
        
        // Verificar que o banco de dados não foi consultado
        _collectionMock.Verify(
            x => x.FindAsync(
                It.IsAny<FilterDefinition<ContentPerformance>>(),
                It.IsAny<FindOptions<ContentPerformance, ContentPerformance>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenCacheMiss_ShouldFetchFromDatabaseAndCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new ContentPerformance
        {
            Id = id,
            CreatorId = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Platform = SocialMediaPlatform.Instagram,
            Views = 800,
            Likes = 300,
            Comments = 80,
            Shares = 25,
            EngagementRate = 35.75m,
            Date = DateTime.UtcNow.AddDays(-10)
        };

        var cacheKey = $"perf:id:{id}";
        _cacheMock.Setup(x => x.GetAsync<ContentPerformance>(cacheKey))
            .ReturnsAsync((ContentPerformance)null);
            
        var mockCursor = new Mock<IAsyncCursor<ContentPerformance>>();
        mockCursor.Setup(x => x.Current).Returns(new List<ContentPerformance> { entity });
        mockCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<ContentPerformance>>(),
                It.IsAny<FindOptions<ContentPerformance, ContentPerformance>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockCursor.Object);
            
        _cacheMock.Setup(x => x.SetAsync(cacheKey, entity, It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        
        // Verificar que o cache foi atualizado
        _cacheMock.Verify(x => x.SetAsync(cacheKey, entity, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetByCreatorIdAsync_WhenCacheHit_ShouldReturnFromCache()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var entities = new List<ContentPerformance>
        {
            new ContentPerformance
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                PostId = Guid.NewGuid(),
                Platform = SocialMediaPlatform.Instagram,
                Views = 1000,
                Likes = 500,
                Comments = 100,
                Shares = 50,
                EngagementRate = 45.50m,
                Date = DateTime.UtcNow.AddDays(-7)
            },
            new ContentPerformance
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                PostId = Guid.NewGuid(),
                Platform = SocialMediaPlatform.Instagram,
                Views = 2000,
                Likes = 800,
                Comments = 150,
                Shares = 75,
                EngagementRate = 51.25m,
                Date = DateTime.UtcNow.AddDays(-5)
            }
        };

        var cacheKey = $"perf:creator:{creatorId}:all";
        
        _cacheMock.Setup(x => x.GetAsync<List<ContentPerformance>>(cacheKey))
            .ReturnsAsync(entities);

        // Act
        var result = await _repository.GetByCreatorIdAsync(creatorId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(x => x.CreatorId == creatorId).Should().BeTrue();
        
        _cacheMock.Verify(x => x.GetAsync<List<ContentPerformance>>(cacheKey), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertPerformance()
    {
        // Arrange
        var entity = new ContentPerformance { /*...*/ };
        _collectionMock.Setup(x => x.InsertOneAsync(
                It.IsAny<ContentPerformance>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        result.Should().BeEquivalentTo(entity);
        _collectionMock.Verify(x => x.InsertOneAsync(entity, null, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplacePerformanceAndInvalidateCache()
    {
        // Arrange
        var entity = new ContentPerformance { Id = Guid.NewGuid(), /*...*/ };
        var cacheKey = $"perf:id:{entity.Id}";
        var replaceResult = new Mock<ReplaceOneResult>();
        replaceResult.Setup(x => x.IsAcknowledged).Returns(true);
        replaceResult.Setup(x => x.ModifiedCount).Returns(1);
        
        _collectionMock.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<ContentPerformance>>(),
                It.IsAny<ContentPerformance>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replaceResult.Object);
        
        // Setup das chaves corretas para o cache
        _cacheMock.Setup(x => x.RemoveAsync($"perf:id:{entity.Id}")).Returns(Task.CompletedTask);
        _cacheMock.Setup(x => x.RemoveAsync($"perf:creator:{entity.CreatorId}:all")).Returns(Task.CompletedTask);
        _cacheMock.Setup(x => x.RemoveAsync($"perf:post:{entity.PostId}:all")).Returns(Task.CompletedTask);

        // Act
        var result = await _repository.UpdateAsync(entity);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(entity);
        _collectionMock.Verify(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<ContentPerformance>>(), entity, It.IsAny<ReplaceOptions>(), default), Times.Once);
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once);
    }

    [Fact]
    public async Task RefreshMetricsAsync_ShouldInvalidateAllCache()
    {
        // Arrange
        _collectionMock.Setup(x => x.UpdateManyAsync(
                It.IsAny<FilterDefinition<ContentPerformance>>(),
                It.IsAny<UpdateDefinition<ContentPerformance>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UpdateResult.Acknowledged(1, 10, BsonValue.Create(1)));
        
        // Setup da chave correta de cache
        _cacheMock.Setup(x => x.RemoveAsync("perf:all")).Returns(Task.CompletedTask);

        // Act
        await _repository.RefreshMetricsAsync();

        // Assert
        _cacheMock.Verify(x => x.RemoveAsync("perf:all"), Times.Once);
    }
} 