using Application.Features.Users.GetUsersQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitterAPI.Application.Abstractions.Messaging;
using TwitterAPI.Domain.Abstractions;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Domain.Entities.Tweet;
using TwitterAPI.Interfaces.Responses;
using TwitterAPI.Responses;
using TwittetAPI.Domain.Abstractions;

namespace Application.Features.Tweets.QueryGetTimeline
{
    public class GetTimelineQueryHandler : IQueryPaginatedHandler<GetTimelineQuery, TimelineDTO>
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly ICacheService _cacheService;

        public GetTimelineQueryHandler(ITweetRepository tweetRepository, ICacheService cacheService)
        {
            this._tweetRepository = tweetRepository;
            this._cacheService = cacheService;
        }

        public async Task<IPaginatedResponse<TimelineDTO>> Handle(GetTimelineQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"timeline:user:{request.dto.userId}:page:{request.dto.pageNumber}:size:{request.dto.pageSize}";

            try
            {
                var cachedData = await _cacheService.GetAsync<CachedTimelineData>(cacheKey, cancellationToken);
                if (cachedData != null)
                {
                    var paginationData = cachedData.Pagination.ToPaginationData();
                    return PaginatedResponse<TimelineDTO>.Success(cachedData.Timeline, paginationData);
                }

                // Cache miss - obtener de repository
                var (tweets, pagination) = await _tweetRepository.GetTimeline(
                    request.dto.userId,
                    request.dto.pageSize,
                    request.dto.pageNumber);

                var timeLine = tweets.Select(tweet => new TimelineDTO
                {
                    content = tweet.Content,
                    username = tweet.User!.UserName
                }).ToList();

                var dataToCache = new CachedTimelineData(timeLine, pagination);

                await _cacheService.SetAsync(cacheKey, dataToCache, TimeSpan.FromMinutes(15), cancellationToken);


                return PaginatedResponse<TimelineDTO>.Success(timeLine, pagination);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
