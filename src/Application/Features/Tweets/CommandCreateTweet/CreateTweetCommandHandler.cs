using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPI.Application.Abstractions.Messaging;
using TwitterAPI.Domain.Abstractions;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Domain.Entities.Tweet;
using TwitterAPI.Interfaces.Responses;
using TwittetAPI.Domain.Abstractions;

namespace Application.Features.Tweets.CommandCreateTweet
{
    public class CreateTweetCommandHandler : ICommandHandler<CreateTweetCommand>
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;

        public CreateTweetCommandHandler(ITweetRepository tweetRepository, IUserRepository userRepository, ICacheService cacheService)
        {
            this._tweetRepository = tweetRepository;
            this._userRepository = userRepository;
            this._cacheService = cacheService;
        }

        public async Task<IResponse> Handle(CreateTweetCommand request, CancellationToken cancellationToken)
        {
            var user = await this._userRepository.GetByIdAsync(request.dto.UserId, cancellationToken);

            if(user == null)
            {
                return Response.Failure(Error.Custom("CREATE_TWEET", "This user doesn't exist"));
            }

            if (request.dto.Content.Length > 280 || request.dto.Content.Length < 3)
            {
                return Response.Failure(Error.Custom("CREATE_TWEET", "Invalid tweet length"));
            }

            var tweet = new Tweet
            {
                Content = request.dto.Content,
                UserId = request.dto.UserId,
            };

            await _tweetRepository.AddAsync(tweet, cancellationToken);

            await this._tweetRepository.SaveAsync(cancellationToken);

            await InvalidateFollowersTimelines(request.dto.UserId, cancellationToken);

            return Response.Success(tweet);
        }

        private async Task InvalidateFollowersTimelines(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var followers = await this._userRepository.GetFollowersByUserId(userId, cancellationToken);

                // Invalidar cache para cada seguidor (todas las páginas y tamaños)
                var invalidationTasks = followers.Select(async followerId =>
                {
                    var pattern = $"timeline:user:{followerId}:*";
                    await this._cacheService.RemoveByPatternAsync(pattern, cancellationToken);
                });

                await Task.WhenAll(invalidationTasks);

                var ownTimelinePattern = $"timeline:user:{userId}:*";
                await this._cacheService.RemoveByPatternAsync(ownTimelinePattern, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log error pero no fallar la creación del tweet
                Console.WriteLine($"Error invalidating cache for user {userId}: {ex.Message}");
            }
        }
    }
}
