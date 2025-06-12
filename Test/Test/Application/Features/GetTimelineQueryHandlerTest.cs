using Application.Dtos;
using Application.Features.Tweets.QueryGetTimeline;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPI.Domain.Entities.Tweet;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Responses;
using TwittetAPI.Domain.Abstractions;
using FluentAssertions;

namespace Test.Application.Features
{
    [TestFixture]
    public class GetTimelineQueryHandlerTests
    {
        private Mock<ITweetRepository> _mockTweetRepository;
        private Mock<ICacheService> _mockCacheService;
        private GetTimelineQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockTweetRepository = new Mock<ITweetRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _handler = new GetTimelineQueryHandler(_mockTweetRepository.Object, _mockCacheService.Object);
        }

        [Test]
        public async Task Handle_CacheHit_ShouldReturnCachedDataWithoutCallingRepository()
        {
            // Arrange
            var getTimelineDto = new GetTimelineDTO
            {
                UserId = 1,
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetTimelineQuery(getTimelineDto);
            var cancellationToken = CancellationToken.None;
            var expectedCacheKey = "timeline:user:1:page:1:size:10";

            var cachedTimeline = new List<TimelineDTO>
            {
                new TimelineDTO { Id = 1, Content = "Cached tweet 1", Username = "user1" },
                new TimelineDTO { Id = 2, Content = "Cached tweet 2", Username = "user2" }
            };

            var cachedPagination = new PaginationData
            {
                TotalCount = 50,
                PageSize = 10,
                PageNumber = 1,
                TotalPages = 5
            };

            var cachedData = new CachedTimelineData(cachedTimeline, cachedPagination);

            _mockCacheService
                .Setup(x => x.GetAsync<CachedTimelineData>(expectedCacheKey, cancellationToken))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data.First().Id.Should().Be(1);
            result.Data.First().Content.Should().Be("Cached tweet 1");
            result.Data.First().Username.Should().Be("user1");

            // Verify cache was checked
            _mockCacheService.Verify(
                x => x.GetAsync<CachedTimelineData>(expectedCacheKey, cancellationToken),
                Times.Once);

            // Verify repository was NOT called (cache hit)
            _mockTweetRepository.Verify(
                x => x.GetTimeline(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            // Verify cache was NOT set (already existed)
            _mockCacheService.Verify(
                x => x.SetAsync(It.IsAny<string>(), It.IsAny<CachedTimelineData>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_CacheMiss_ShouldFetchFromRepositoryAndCacheResult()
        {
            // Arrange
            var getTimelineDto = new GetTimelineDTO
            {
                UserId = 1,
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetTimelineQuery(getTimelineDto);
            var cancellationToken = CancellationToken.None;
            var expectedCacheKey = "timeline:user:1:page:1:size:10";

            var mockTweets = new List<Tweet>
            {
                new Tweet { Id = 1, Content = "Tweet 1", User = new User { UserName = "user1" }, UserId = 1, },
                new Tweet { Id = 2, Content = "Tweet 2", User = new User { UserName = "user2" }, UserId = 2, }
            };

            var mockPagination = new PaginationData
            {
                TotalCount = 50,
                PageSize = 10,
                PageNumber = 1,
                TotalPages = 5
            };

            // Cache miss
            _mockCacheService
                .Setup(x => x.GetAsync<CachedTimelineData>(expectedCacheKey, cancellationToken))
                .ReturnsAsync((CachedTimelineData?)null);

            _mockTweetRepository
                .Setup(x => x.GetTimeline(getTimelineDto.UserId, getTimelineDto.PageSize, getTimelineDto.PageNumber))
                .ReturnsAsync((mockTweets, mockPagination));

            _mockCacheService
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CachedTimelineData>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.PaginationData.Should().Be(mockPagination);

            // Verify data transformation
            var timelineDtos = result.Data.ToList();
            timelineDtos[0].Id.Should().Be(1);
            timelineDtos[0].Content.Should().Be("Tweet 1");
            timelineDtos[0].Username.Should().Be("user1");
            timelineDtos[1].Id.Should().Be(2);
            timelineDtos[1].Content.Should().Be("Tweet 2");
            timelineDtos[1].Username.Should().Be("user2");

            // Verify cache was checked
            _mockCacheService.Verify(
                x => x.GetAsync<CachedTimelineData>(expectedCacheKey, cancellationToken),
                Times.Once);

            // Verify repository was called
            _mockTweetRepository.Verify(
                x => x.GetTimeline(getTimelineDto.UserId, getTimelineDto.PageSize, getTimelineDto.PageNumber),
                Times.Once);

            // Verify result was cached with 15 minutes expiration
            _mockCacheService.Verify(
                x => x.SetAsync(
                    expectedCacheKey,
                    It.Is<CachedTimelineData>(data => data.Timeline.Count == 2),
                    TimeSpan.FromMinutes(15),
                    cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task Handle_EmptyTimeline_ShouldReturnEmptyResultAndCache()
        {
            // Arrange
            var getTimelineDto = new GetTimelineDTO
            {
                UserId = 1,
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetTimelineQuery(getTimelineDto);
            var cancellationToken = CancellationToken.None;

            var emptyTweets = new List<Tweet>();
            var mockPagination = new PaginationData
            {
                TotalCount = 0,
                PageSize = 10,
                PageNumber = 1,
                TotalPages = 0
            };

            _mockCacheService
                .Setup(x => x.GetAsync<CachedTimelineData>(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((CachedTimelineData?)null);

            _mockTweetRepository
                .Setup(x => x.GetTimeline(getTimelineDto.UserId, getTimelineDto.PageSize, getTimelineDto.PageNumber))
                .ReturnsAsync((emptyTweets, mockPagination));

            _mockCacheService
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CachedTimelineData>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.PaginationData.TotalCount.Should().Be(0);

            // Verify empty result was still cached
            _mockCacheService.Verify(
                x => x.SetAsync(
                    It.IsAny<string>(),
                    It.Is<CachedTimelineData>(data => data.Timeline.Count == 0),
                    TimeSpan.FromMinutes(15),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_RepositoryThrowsException_ShouldReturnFailureResponse()
        {
            // Arrange
            var getTimelineDto = new GetTimelineDTO
            {
                UserId = 1,
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetTimelineQuery(getTimelineDto);
            var cancellationToken = CancellationToken.None;

            _mockCacheService
                .Setup(x => x.GetAsync<CachedTimelineData>(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((CachedTimelineData?)null);

            _mockTweetRepository
                .Setup(x => x.GetTimeline(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(x => x != null &&
                x.code == "TIMELINE" &&
                x.description == "Error in the timeline");

            // Verify cache was checked
            _mockCacheService.Verify(
                x => x.GetAsync<CachedTimelineData>(It.IsAny<string>(), cancellationToken),
                Times.Once);

            // Verify repository was called
            _mockTweetRepository.Verify(
                x => x.GetTimeline(getTimelineDto.UserId, getTimelineDto.PageSize, getTimelineDto.PageNumber),
                Times.Once);

            // Verify nothing was cached due to error
            _mockCacheService.Verify(
                x => x.SetAsync(It.IsAny<string>(), It.IsAny<CachedTimelineData>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_CacheServiceThrowsException_ShouldReturnFailureResponse()
        {
            // Arrange
            var getTimelineDto = new GetTimelineDTO
            {
                UserId = 1,
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetTimelineQuery(getTimelineDto);
            var cancellationToken = CancellationToken.None;

            _mockCacheService
                .Setup(x => x.GetAsync<CachedTimelineData>(It.IsAny<string>(), cancellationToken))
                .ThrowsAsync(new InvalidOperationException("Cache error"));

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(x => x != null &&
                x.code == "TIMELINE" &&
                x.description == "Error in the timeline");

            // Verify repository was NOT called due to cache error
            _mockTweetRepository.Verify(
                x => x.GetTimeline(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_DifferentPageSizes_ShouldUseDifferentCacheKeys()
        {
            // Arrange
            var getTimelineDto1 = new GetTimelineDTO { UserId = 1, PageSize = 10, PageNumber = 1 };
            var getTimelineDto2 = new GetTimelineDTO { UserId = 1, PageSize = 20, PageNumber = 1 };

            var query1 = new GetTimelineQuery(getTimelineDto1);
            var query2 = new GetTimelineQuery(getTimelineDto2);
            var cancellationToken = CancellationToken.None;

            var expectedCacheKey1 = "timeline:user:1:page:1:size:10";
            var expectedCacheKey2 = "timeline:user:1:page:1:size:20";

            _mockCacheService
                .Setup(x => x.GetAsync<CachedTimelineData>(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((CachedTimelineData?)null);

            _mockTweetRepository
                .Setup(x => x.GetTimeline(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<Tweet>(), new PaginationData()));

            _mockCacheService
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CachedTimelineData>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(query1, cancellationToken);
            await _handler.Handle(query2, cancellationToken);

            // Assert
            _mockCacheService.Verify(
                x => x.GetAsync<CachedTimelineData>(expectedCacheKey1, cancellationToken),
                Times.Once);

            _mockCacheService.Verify(
                x => x.GetAsync<CachedTimelineData>(expectedCacheKey2, cancellationToken),
                Times.Once);

            _mockCacheService.Verify(
                x => x.SetAsync(expectedCacheKey1, It.IsAny<CachedTimelineData>(), TimeSpan.FromMinutes(15), cancellationToken),
                Times.Once);

            _mockCacheService.Verify(
                x => x.SetAsync(expectedCacheKey2, It.IsAny<CachedTimelineData>(), TimeSpan.FromMinutes(15), cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task Handle_SetCacheThrowsException_ShouldStillReturnSuccessfulResult()
        {
            // Arrange
            var getTimelineDto = new GetTimelineDTO
            {
                UserId = 1,
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetTimelineQuery(getTimelineDto);
            var cancellationToken = CancellationToken.None;

            var mockTweets = new List<Tweet>
            {
                new Tweet { Id = 1, Content = "Tweet 1", User = new User { UserName = "user1" }, UserId = 1 }
            };

            var mockPagination = new PaginationData { TotalCount = 1, PageSize = 10, PageNumber = 1, TotalPages = 1 };

            _mockCacheService
                .Setup(x => x.GetAsync<CachedTimelineData>(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((CachedTimelineData?)null);

            _mockTweetRepository
                .Setup(x => x.GetTimeline(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((mockTweets, mockPagination));

            _mockCacheService
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CachedTimelineData>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Cache set error"));

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert - Should still return failure due to try-catch around entire method
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(x => x != null &&
                x.code == "TIMELINE" &&
                x.description == "Error in the timeline");
        }

        [TearDown]
        public void TearDown()
        {
            _mockTweetRepository = null;
            _mockCacheService = null;
            _handler = null;
        }
    }
}
