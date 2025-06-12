using TwitterAPI.Application.Abstractions.Messaging;
using TwitterAPI.Domain.Abstractions;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Interfaces.Responses;

namespace Application.Features.Follow.CommandFollowUser
{
    public class FollowUserCommandHandler : ICommandHandler<FollowUserCommand>
    {
        private readonly IUserFollowRepository _userFollowRepository;

        public FollowUserCommandHandler(IUserFollowRepository userFollowRepository)
        {
            this._userFollowRepository = userFollowRepository;
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

            return Response.Success();
        }
    }
}
