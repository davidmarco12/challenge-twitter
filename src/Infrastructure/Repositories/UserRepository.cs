using Domain.Extentions;
using Microsoft.EntityFrameworkCore;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Domain.Entities.Tweet;
using TwitterAPI.Responses;

namespace Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(List<User>, PaginationData)> GetPaginatedUsers(int pageSize, int pageNumber)
        {
            var (timeline, pagination) = await this.DbContext.User.AsQueryable()
                .ToPaginatedListAsync(pageNumber, pageSize);

            return (timeline, pagination);
        }

        public async Task<List<int>> GetFollowersByUserId(int userId, CancellationToken cancellationToken)
        {
            return await this.DbContext.UserFollows
                .Where(uf => uf.FollowingId == userId)
                .Select(uf => uf.FollowerId)
                .ToListAsync(cancellationToken);
        }
    }
}
