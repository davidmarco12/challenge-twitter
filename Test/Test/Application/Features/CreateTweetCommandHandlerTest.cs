using Application.Features.Tweets.CommandCreateTweet;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPI.Domain.Entities.Tweet;
using TwitterAPI.Domain.Entities;
using TwittetAPI.Domain.Abstractions;
using Application.DTOs;

namespace Test.Application.Features
{
    [TestFixture]
    public class CreateTweetCommandHandlerTests
    {
        private Mock<ITweetRepository> _mockTweetRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ICacheService> _mockCacheService;
        private CreateTweetCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockTweetRepository = new Mock<ITweetRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _handler = new CreateTweetCommandHandler(
                _mockTweetRepository.Object,
                _mockUserRepository.Object,
                _mockCacheService.Object);
        }

        [Test]
        public async Task Handle_ValidTweet_ShouldCreateTweetSuccessfully()
        {
            // Arrange
            var createTweetDto = new CreateTweetDTO
            {
                UserId = 1,
                Content = "This is a valid tweet content!"
            };

            var command = new CreateTweetCommand(createTweetDto);
            var cancellationToken = CancellationToken.None;

            var mockUser = new User { Id = 1, UserName = "testuser" };
            var mockFollowers = new List<int> { 2, 3 }; // Usuarios 2 y 3 siguen al usuario 1

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync(mockUser);

            _mockUserRepository
                .Setup(x => x.GetFollowersByUserId(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync(mockFollowers);

            _mockTweetRepository
                .Setup(x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockTweetRepository
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
            _mockUserRepository.Verify(
                x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockUserRepository.Verify(
                x => x.GetFollowersByUserId(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockTweetRepository.Verify(
                x => x.AddAsync(It.Is<Tweet>(t =>
                    t.Content == createTweetDto.Content &&
                    t.UserId == createTweetDto.UserId),
                    cancellationToken),
                Times.Once);

            _mockTweetRepository.Verify(
                x => x.SaveAsync(cancellationToken),
                Times.Once);

            // Verify cache invalidation calls
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync("timeline:user:2:*", cancellationToken),
                Times.Once);

            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync("timeline:user:3:*", cancellationToken),
                Times.Once);

            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync("timeline:user:1:*", cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task Handle_UserDoesNotExist_ShouldReturnFailureResponse()
        {
            // Arrange
            var createTweetDto = new CreateTweetDTO
            {
                UserId = 999,
                Content = "This is a valid tweet content!"
            };

            var command = new CreateTweetCommand(createTweetDto);
            var cancellationToken = CancellationToken.None;

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(x => x != null &&
                x.code == "CREATE_TWEET" &&
                x.description == "This user doesn't exist");

            // Verify that user repository was called but tweet repository and cache service were NOT called
            _mockUserRepository.Verify(
                x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockUserRepository.Verify(
                x => x.GetFollowersByUserId(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockTweetRepository.Verify(
                x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockTweetRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_TweetTooLong_ShouldReturnFailureResponse()
        {
            // Arrange
            var longContent = new string('A', 281); // 281 characters, exceeds 280 limit
            var createTweetDto = new CreateTweetDTO
            {
                UserId = 1,
                Content = longContent
            };

            var command = new CreateTweetCommand(createTweetDto);
            var cancellationToken = CancellationToken.None;

            var mockUser = new User { Id = 1, UserName = "testuser" };

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync(mockUser);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(x => x != null &&
                x.code == "CREATE_TWEET" &&
                x.description == "Invalid tweet length");

            // Verify user was checked but tweet was not created and cache was not invalidated
            _mockUserRepository.Verify(
                x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockUserRepository.Verify(
                x => x.GetFollowersByUserId(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockTweetRepository.Verify(
                x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockTweetRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_TweetTooShort_ShouldReturnFailureResponse()
        {
            // Arrange
            var createTweetDto = new CreateTweetDTO
            {
                UserId = 1,
                Content = "Hi" // 2 characters, less than 3 minimum
            };

            var command = new CreateTweetCommand(createTweetDto);
            var cancellationToken = CancellationToken.None;

            var mockUser = new User { Id = 1, UserName = "testuser" };

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync(mockUser);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(x => x != null &&
                x.code == "CREATE_TWEET" &&
                x.description == "Invalid tweet length");

            // Verify user was checked but tweet was not created and cache was not invalidated
            _mockUserRepository.Verify(
                x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockUserRepository.Verify(
                x => x.GetFollowersByUserId(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockTweetRepository.Verify(
                x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockTweetRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        [TestCase(3, "Hii")]    // Minimum valid length
        [TestCase(280, "This content is exactly 280 characters long and should be valid for a tweet content. We need to make sure this is exactly the right length to test the boundary condition properly. Let's count: 123456789012345678901234567890123456789012345678901234567890123456789012345678901234567")]
        public async Task Handle_ValidTweetLengths_ShouldCreateTweetSuccessfully(int expectedLength, string content)
        {
            // Arrange

            var createTweetDto = new CreateTweetDTO
            {
                UserId = 1,
                Content = content
            };

            var command = new CreateTweetCommand(createTweetDto);
            var cancellationToken = CancellationToken.None;

            var mockUser = new User { Id = 1, UserName = "testuser" };
            var mockFollowers = new List<int> { 2 };

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync(mockUser);

            _mockUserRepository
                .Setup(x => x.GetFollowersByUserId(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync(mockFollowers);

            _mockTweetRepository
                .Setup(x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockTweetRepository
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

            // Verify all repository calls were made
            _mockUserRepository.Verify(
                x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockUserRepository.Verify(
                x => x.GetFollowersByUserId(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockTweetRepository.Verify(
                x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockTweetRepository.Verify(
                x => x.SaveAsync(cancellationToken),
                Times.Once);

            // Verify cache invalidation was called
            _mockCacheService.Verify(
                x => x.RemoveByPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.AtLeast(1)); // At least once for follower + once for own timeline
        }

        [Test]
        public void Handle_TweetRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var createTweetDto = new CreateTweetDTO
            {
                UserId = 1,
                Content = "Valid tweet content"
            };

            var command = new CreateTweetCommand(createTweetDto);
            var cancellationToken = CancellationToken.None;

            var mockUser = new User { Id = 1, UserName = "testuser" };

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken))
                .ReturnsAsync(mockUser);

            _mockTweetRepository
                .Setup(x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, cancellationToken));

            // Verify calls up to the point of exception
            _mockUserRepository.Verify(
                x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockTweetRepository.Verify(
                x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockTweetRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Handle_UserRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var createTweetDto = new CreateTweetDTO
            {
                UserId = 1,
                Content = "Valid tweet content"
            };

            var command = new CreateTweetCommand(createTweetDto);
            var cancellationToken = CancellationToken.None;

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken))
                .ThrowsAsync(new InvalidOperationException("User database error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, cancellationToken));

            // Verify only user repository was called
            _mockUserRepository.Verify(
                x => x.GetByIdAsync(createTweetDto.UserId, cancellationToken),
                Times.Once);

            _mockTweetRepository.Verify(
                x => x.AddAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockTweetRepository.Verify(
                x => x.SaveAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TearDown]
        public void TearDown()
        {
            _mockTweetRepository = null;
            _mockUserRepository = null;
            _handler = null;
        }
    }
}
