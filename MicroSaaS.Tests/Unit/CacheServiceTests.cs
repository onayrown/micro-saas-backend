using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using FluentAssertions;
using Xunit;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace MicroSaaS.Tests.Unit;

public class CacheServiceTests
{
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly ICacheService _cacheService;
    private readonly DistributedCacheEntryOptions _defaultOptions;

    public CacheServiceTests()
    {
        _distributedCacheMock = new Mock<IDistributedCache>();
        _cacheService = new RedisCacheService(_distributedCacheMock.Object);
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
    }

    [Fact]
    public async Task GetAsync_WhenKeyExists_ShouldReturnDeserializedValue()
    {
        // Arrange
        var key = "test_key";
        var testObject = new TestObject { Id = 1, Name = "Test" };
        var serializedValue = JsonSerializer.Serialize(testObject);
        var bytesValue = Encoding.UTF8.GetBytes(serializedValue);

        _distributedCacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytesValue);

        // Act
        var result = await _cacheService.GetAsync<TestObject>(key);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(testObject.Id);
        result.Name.Should().Be(testObject.Name);

        _distributedCacheMock.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenKeyDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        var key = "non_existent_key";

        _distributedCacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null);

        // Act
        var result = await _cacheService.GetAsync<TestObject>(key);

        // Assert
        result.Should().BeNull();

        _distributedCacheMock.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetAsync_ShouldSerializeAndStoreValue()
    {
        // Arrange
        var key = "test_key";
        var testObject = new TestObject { Id = 1, Name = "Test" };
        var expiration = TimeSpan.FromMinutes(15);

        _distributedCacheMock.Setup(x => x.SetAsync(
                key,
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiration),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.SetAsync(key, testObject, expiration);

        // Assert
        _distributedCacheMock.Verify(x => x.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiration),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithNoExpiration_ShouldUseDefaultExpiration()
    {
        // Arrange
        var key = "test_key";
        var testObject = new TestObject { Id = 1, Name = "Test" };

        _distributedCacheMock.Setup(x => x.SetAsync(
                key,
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow.HasValue),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.SetAsync(key, testObject);

        // Assert
        _distributedCacheMock.Verify(x => x.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow.HasValue),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallRemoveOnDistributedCache()
    {
        // Arrange
        var key = "test_key";

        _distributedCacheMock.Setup(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _distributedCacheMock.Verify(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyExists_ShouldReturnTrue()
    {
        // Arrange
        var key = "test_key";

        _distributedCacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[] { 1, 2, 3 });

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeTrue();

        _distributedCacheMock.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var key = "non_existent_key";

        _distributedCacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null);

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeFalse();

        _distributedCacheMock.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrSetAsync_WhenKeyExists_ShouldReturnCachedValue()
    {
        // Arrange
        var key = "test_key";
        var testObject = new TestObject { Id = 1, Name = "Test" };
        var serializedValue = JsonSerializer.Serialize(testObject);
        var bytesValue = Encoding.UTF8.GetBytes(serializedValue);
        var factoryWasCalled = false;

        _distributedCacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytesValue);

        // Act
        var result = await _cacheService.GetOrSetAsync(key, async () => 
        {
            factoryWasCalled = true;
            return new TestObject { Id = 2, Name = "Different" };
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(testObject.Id);
        result.Name.Should().Be(testObject.Name);
        factoryWasCalled.Should().BeFalse();

        _distributedCacheMock.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        _distributedCacheMock.Verify(x => x.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task GetOrSetAsync_WhenKeyDoesNotExist_ShouldCallFactoryAndSetCache()
    {
        // Arrange
        var key = "non_existent_key";
        var testObject = new TestObject { Id = 2, Name = "Factory Generated" };
        var expiration = TimeSpan.FromMinutes(15);
        var factoryWasCalled = false;

        _distributedCacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null);

        _distributedCacheMock.Setup(x => x.SetAsync(
                key,
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiration),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cacheService.GetOrSetAsync(key, async () => 
        {
            factoryWasCalled = true;
            return testObject;
        }, expiration);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(testObject.Id);
        result.Name.Should().Be(testObject.Name);
        factoryWasCalled.Should().BeTrue();

        _distributedCacheMock.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        _distributedCacheMock.Verify(x => x.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiration),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
} 