using Moq;
using TwitterAPI.Domain.Entities;
using Application.Features.Follow.CommandFollowUser;
using Application.DTOs;
using FluentAssertions;
using TwitterAPI.Domain.Abstractions;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace Test.Application.Features
{
    [TestFixture]
    public class FollowUserCommandHandlerTests
    {
        private Mock<IUserFollowRepository> _mockUserFollowRepository;
        private Mock<ICacheService> _mockCacheService;
        private FollowUserCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockUserFollowRepository = new Mock<IUserFollowRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _handler = new FollowUserCommandHandler(_mockUserFollowRepository.Object, _mockCacheService.Object);
        }

        [Test]
        public async Task Handle_ValidFollowRequest_ShouldReturnSuccessResponse()
        {
            // Arrange
            var followDto = new FollowUserDTO
            {
                FollowerId = 1,
                FollowingId = 2
            };

            var command = new FollowUserCommand(followDto);
            var cancellationToken = CancellationToken.None;

            _mockUserFollowRepository
                .Setup(x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUserFollowRepository
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockCacheService
                .Setup(x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();

            // Verify repository calls
            _mockUserFollowRepository.Verify(
                x => x.AddAsync(It.Is<UserFollow>(uf =>
                    uf.FollowerId == followDto.FollowerId &&
                    uf.FollowingId == followDto.FollowingId),
                    cancellationToken),
                Times.Once);

            _mockUserFollowRepository.Verify(
                x => x.SaveAsync(cancellationToken),
                Times.Once);

            // Verify cache invalidation for the follower's timeline
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync("timeline:user:1:*", cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task Handle_UserTriesToFollowThemself_ShouldReturnFailureResponse()
        {
            // Arrange
            var followDto = new FollowUserDTO
            {
                FollowerId = 1,
                FollowingId = 1 // Mismo usuario
            };

            var command = new FollowUserCommand(followDto);
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(Error.FollowError);

            // Verify that repository methods are NOT called
            _mockUserFollowRepository.Verify(
                x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockUserFollowRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            // Verify that cache invalidation is NOT called
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var followDto = new FollowUserDTO
            {
                FollowerId = 1,
                FollowingId = 2
            };

            var command = new FollowUserCommand(followDto);
            var cancellationToken = CancellationToken.None;

            _mockUserFollowRepository
                .Setup(x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, cancellationToken));

            // Verify AddAsync was called but SaveAsync was not
            _mockUserFollowRepository.Verify(
                x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockUserFollowRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            // Verify cache invalidation was NOT called due to exception
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Handle_SaveAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var followDto = new FollowUserDTO
            {
                FollowerId = 1,
                FollowingId = 2
            };

            var command = new FollowUserCommand(followDto);
            var cancellationToken = CancellationToken.None;

            _mockUserFollowRepository
                .Setup(x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUserFollowRepository
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Save failed"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, cancellationToken));

            // Verify both methods were called
            _mockUserFollowRepository.Verify(
                x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockUserFollowRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            // Verify cache invalidation was NOT called due to exception
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_CacheInvalidationFails_ShouldStillReturnSuccess()
        {
            // Arrange
            var followDto = new FollowUserDTO
            {
                FollowerId = 1,
                FollowingId = 2
            };

            var command = new FollowUserCommand(followDto);
            var cancellationToken = CancellationToken.None;

            _mockUserFollowRepository
                .Setup(x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUserFollowRepository
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockCacheService
                .Setup(x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Cache service error"));

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();

            // Verify repository operations completed successfully
            _mockUserFollowRepository.Verify(
                x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockUserFollowRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            // Verify cache invalidation was attempted
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync("timeline:user:1:*", cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task Handle_DifferentFollowerIds_ShouldInvalidateCorrectCache()
        {
            // Arrange
            var followDto = new FollowUserDTO
            {
                FollowerId = 5,
                FollowingId = 10
            };

            var command = new FollowUserCommand(followDto);
            var cancellationToken = CancellationToken.None;

            _mockUserFollowRepository
                .Setup(x => x.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUserFollowRepository
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockCacheService
                .Setup(x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify cache invalidation for the specific follower (user 5)
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync("timeline:user:5:*", cancellationToken),
                Times.Once);

            // Verify cache is NOT invalidated for the followed user (user 10)
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync("timeline:user:10:*", It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TearDown]
        public void TearDown()
        {
            _mockUserFollowRepository = null;
            _mockCacheService = null;
            _handler = null;
        }
    }
}