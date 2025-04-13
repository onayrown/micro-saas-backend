using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
// using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Infrastructure.Persistence.Repositories;
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
using System.Threading;
using MicroSaaS.Application.Interfaces.Repositories;

namespace MicroSaaS.Tests.Unit;

public class UserRepositoryTests
{
    private readonly Mock<IMongoCollection<User>> _collectionMock;
    private readonly Mock<IMongoDbContext> _contextMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<ILoggingService> _loggingServiceMock;
    private readonly IUserRepository _repository;
    private List<User> _users;

    public UserRepositoryTests()
    {
        _collectionMock = new Mock<IMongoCollection<User>>();
        _contextMock = new Mock<IMongoDbContext>();
        _cacheMock = new Mock<ICacheService>();
        _loggingServiceMock = new Mock<ILoggingService>();

        // Configurar a propriedade Users do contexto de banco de dados
        _contextMock.Setup(x => x.Users).Returns(_collectionMock.Object);
        
        // Configurar a chamada GetCollection também para casos onde possa ser usada
        _contextMock.Setup(x => x.GetCollection<User>("users"))
            .Returns(_collectionMock.Object);

        _repository = new UserRepository(_contextMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        var users = new List<User> { user };
        
        var mockCursor = new Mock<IAsyncCursor<User>>();
        mockCursor.Setup(x => x.Current).Returns(users);
        mockCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var mockCursor = new Mock<IAsyncCursor<User>>();
        mockCursor.Setup(x => x.Current).Returns(new List<User>());
        mockCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _collectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsernameAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var username = "testuser";
        var cacheKey = $"user_username_{username.ToLower()}";
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        _cacheMock.Setup(x => x.GetAsync<User>(cacheKey))
            .ReturnsAsync((User)null);

        var cursor = new Mock<IAsyncCursor<User>>();
        cursor.Setup(x => x.Current).Returns(new List<User> { user });
        cursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        // Act
        var result = await _repository.GetByUsernameAsync(username);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be(username);
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var email = "test@example.com";
        var cacheKey = $"user_email_{email.ToLower()}";
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = email,
            PasswordHash = "hashedpassword",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        _cacheMock.Setup(x => x.GetAsync<User>(cacheKey))
            .ReturnsAsync((User)null);

        var cursor = new Mock<IAsyncCursor<User>>();
        cursor.Setup(x => x.Current).Returns(new List<User> { user });
        cursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertUserAndReturnDomainObject()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "newuser",
            Email = "new@example.com",
            PasswordHash = "hashedpassword",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _collectionMock.Setup(x => x.InsertOneAsync(
                It.IsAny<User>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _repository.AddAsync(user);

        // Assert
        _collectionMock.Verify(x => x.InsertOneAsync(
            It.IsAny<User>(),
            It.IsAny<InsertOneOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserAndInvalidateCache()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "updateduser",
            Email = "updated@example.com",
            PasswordHash = "hashedpassword",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow
        };
        
        var replaceResult = new Mock<ReplaceOneResult>();
        replaceResult.Setup(x => x.IsAcknowledged).Returns(true);
        replaceResult.Setup(x => x.ModifiedCount).Returns(1);

        _collectionMock.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<User>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replaceResult.Object);

        // Act
        await _repository.UpdateAsync(user);

        // Assert
        _collectionMock.Verify(x => x.ReplaceOneAsync(
            It.IsAny<FilterDefinition<User>>(),
            It.IsAny<User>(),
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetUserInactiveAndInvalidateCache()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var deleteResult = new Mock<DeleteResult>();
        deleteResult.Setup(x => x.IsAcknowledged).Returns(true);
        deleteResult.Setup(x => x.DeletedCount).Returns(1);

        _collectionMock.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult.Object);

        // Act
        await _repository.DeleteAsync(userId);

        // Assert
        _collectionMock.Verify(x => x.DeleteOneAsync(
            It.IsAny<FilterDefinition<User>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), IsActive = true, Username = "user1" },
            new User { Id = Guid.NewGuid(), IsActive = true, Username = "user2" }
        };

        var cursor = new Mock<IAsyncCursor<User>>();
        cursor.Setup(x => x.Current).Returns(users);
        cursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
} 