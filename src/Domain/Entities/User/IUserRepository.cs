using TwitterAPI.Domain.Abstractions;
using TwitterAPI.Responses;

namespace TwitterAPI.Domain.Entities
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<(List<User>, PaginationData)> GetPaginatedUsers(int pageSize, int pageNumber);

        Task<List<int>> GetFollowersByUserId(int userId, CancellationToken cancellationToken);
    }
}
