using Application.DTOs;
using Application.Features.Users.GetUsersQuery;
using FluentAssertions;
using Moq;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Responses;

namespace Test.Application.Features
{
    [TestFixture]
    public class GetUsersQueryHandlerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private GetUsersQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _handler = new GetUsersQueryHandler(_mockUserRepository.Object);
        }

        [Test]
        public async Task Handle_ValidRequest_ShouldReturnPaginatedUsersSuccessfully()
        {
            // Arrange
            var getUsersDto = new GetUsersDTO
            {
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetUsersQuery(getUsersDto);
            var cancellationToken = CancellationToken.None;

            var mockUsers = new List<User>
            {
                new User { Id = 1, UserName = "user1" },
                new User { Id = 2, UserName = "user2" },
                new User { Id = 3, UserName = "user3" }
            };

            var mockPaginationData = new PaginationData
            {
                TotalCount = 50,
                PageSize = 10,
                PageNumber = 1,
                TotalPages = 5
            };

            _mockUserRepository
                .Setup(x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber))
                .ReturnsAsync((mockUsers, mockPaginationData));

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.PaginationData.Should().Be(mockPaginationData);

            // Verify data transformation
            var userDtos = result.Data.ToList();
            userDtos[0].Id.Should().Be(1);
            userDtos[0].username.Should().Be("user1");
            userDtos[1].Id.Should().Be(2);
            userDtos[1].username.Should().Be("user2");
            userDtos[2].Id.Should().Be(3);
            userDtos[2].username.Should().Be("user3");

            // Verify repository call
            _mockUserRepository.Verify(
                x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber),
                Times.Once);
        }

        [Test]
        public async Task Handle_EmptyUsersList_ShouldReturnEmptyPaginatedResponse()
        {
            // Arrange
            var getUsersDto = new GetUsersDTO
            {
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetUsersQuery(getUsersDto);
            var cancellationToken = CancellationToken.None;

            var emptyUsers = new List<User>();
            var mockPaginationData = new PaginationData
            {
                TotalCount = 0,
                PageSize = 10,
                PageNumber = 1,
                TotalPages = 0
            };

            _mockUserRepository
                .Setup(x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber))
                .ReturnsAsync((emptyUsers, mockPaginationData));

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.PaginationData.Should().Be(mockPaginationData);
            result.PaginationData.TotalCount.Should().Be(0);

            // Verify repository call
            _mockUserRepository.Verify(
                x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber),
                Times.Once);
        }

        [Test]
        public async Task Handle_NullUsersList_ShouldReturnEmptyPaginatedResponse()
        {
            // Arrange
            var getUsersDto = new GetUsersDTO
            {
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetUsersQuery(getUsersDto);
            var cancellationToken = CancellationToken.None;

            var mockPaginationData = new PaginationData
            {
                TotalCount = 0,
                PageSize = 10,
                PageNumber = 1,
                TotalPages = 0
            };

            _mockUserRepository
                .Setup(x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber))
                .ReturnsAsync(((List<User>?)null, mockPaginationData));

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.PaginationData.Should().Be(mockPaginationData);

            // Verify repository call
            _mockUserRepository.Verify(
                x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber),
                Times.Once);
        }

        [Test]
        public void Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var getUsersDto = new GetUsersDTO
            {
                PageSize = 10,
                PageNumber = 1
            };

            var query = new GetUsersQuery(getUsersDto);
            var cancellationToken = CancellationToken.None;

            _mockUserRepository
                .Setup(x => x.GetPaginatedUsers(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(query, cancellationToken));

            // Verify repository was called
            _mockUserRepository.Verify(
                x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber),
                Times.Once);
        }

        [Test]
        public async Task Handle_LargePageSize_ShouldHandleCorrectly()
        {
            // Arrange
            var getUsersDto = new GetUsersDTO
            {
                PageSize = 100,
                PageNumber = 2
            };

            var query = new GetUsersQuery(getUsersDto);
            var cancellationToken = CancellationToken.None;

            var mockUsers = Enumerable.Range(101, 50)
                .Select(i => new User { Id = i, UserName = $"user{i}" })
                .ToList();

            var mockPaginationData = new PaginationData
            {
                TotalCount = 500,
                PageSize = 100,
                PageNumber = 2,
                TotalPages = 5
            };

            _mockUserRepository
                .Setup(x => x.GetPaginatedUsers(getUsersDto.PageSize, getUsersDto.PageNumber))
                .ReturnsAsync((mockUsers, mockPaginationData));

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(50);
            result.PaginationData.PageNumber.Should().Be(2);
            result.PaginationData.PageSize.Should().Be(100);
            result.PaginationData.TotalCount.Should().Be(500);
            result.PaginationData.TotalPages.Should().Be(5);

            // Verify first and last users
            var userDtos = result.Data.ToList();
            userDtos.First().Id.Should().Be(101);
            userDtos.First().username.Should().Be("user101");
            userDtos.Last().Id.Should().Be(150);
            userDtos.Last().username.Should().Be("user150");

            // Verify repository call with correct parameters
            _mockUserRepository.Verify(
                x => x.GetPaginatedUsers(100, 2),
                Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            _mockUserRepository = null;
            _handler = null;
        }
    }
}
