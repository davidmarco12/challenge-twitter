using TwitterAPI.Domain.Entities;

namespace Infrastructure.Repositories
{
    public class UserFollowRepository : BaseRepository<UserFollow>, IUserFollowRepository
    {
        public UserFollowRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
