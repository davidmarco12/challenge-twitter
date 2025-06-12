using Domain.Interfaces;
using TwitterAPI.Responses;

namespace TwitterAPI.Domain.Entities.Tweet
{
    public interface ITweetRepository : IBaseRepository<Tweet>
    {
        Task<(List<Tweet>, PaginationData)> GetTimeline(int userId, int pageSize, int pageNumber);
    }
}
