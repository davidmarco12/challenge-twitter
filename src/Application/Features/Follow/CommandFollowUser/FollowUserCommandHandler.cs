using Domain.Interfaces;
using TwitterAPI.Application.Abstractions.Messaging;
using TwitterAPI.Domain.Abstractions;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Interfaces.Responses;

namespace Application.Features.Follow.CommandFollowUser
{
    public class FollowUserCommandHandler : ICommandHandler<FollowUserCommand>
    {
        private readonly IUserFollowRepository _userFollowRepository;
        private readonly ICacheService _cacheService;


        public FollowUserCommandHandler(IUserFollowRepository userFollowRepository, ICacheService cacheService)
        {
            this._userFollowRepository = userFollowRepository;
            this._cacheService = cacheService;
        }

        public async Task<IResponse> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            if (request.dto.FollowerId == request.dto.FollowingId)
            {
                return Response.Failure(Error.FollowError);
            }

            var follow = new UserFollow
            {
                FollowingId = request.dto.FollowingId,
                FollowerId = request.dto.FollowerId 
            };

            await this._userFollowRepository.AddAsync(follow, cancellationToken);

            await this._userFollowRepository.SaveAsync(cancellationToken);

            await InvalidateFollowerTimeline(request.dto.FollowerId, cancellationToken);

            return Response.Success();
        }

        private async Task InvalidateFollowerTimeline(int followerId, CancellationToken cancellationToken)
        {
            try
            {
                // Solo invalidar el timeline del usuario que siguió a alguien
                var timelinePattern = $"timeline:user:{followerId}:*";
                await _cacheService.RemoveByPatternAsync(timelinePattern, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error invalidating cache for user {followerId}: {ex.Message}");
            }
        }
    }
}
