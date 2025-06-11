using Domain.Extentions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitterAPI.Domain.Entities.Tweet;
using TwitterAPI.Responses;

namespace Infrastructure.Repositories
{
    public class TweetRepository : BaseRepository<Tweet>, ITweetRepository
    {
        public TweetRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(List<Tweet>, PaginationData)> GetTimeline(int userId, int pageSize, int pageNumber)
        {
            var (timeline, pagination) = await DbContext.Tweet.AsQueryable()
                .Include(t => t.User)
                .Where(t => DbContext.UserFollows
                    .Where(uf => uf.FollowerId == userId)
                    .Select(uf => uf.FollowingId)
                    .Contains(t.UserId))
                .OrderByDescending(t => t.CreationDate)
                .ToPaginatedListAsync(pageNumber, pageSize);

            return (timeline, pagination);
        }
    }
}
