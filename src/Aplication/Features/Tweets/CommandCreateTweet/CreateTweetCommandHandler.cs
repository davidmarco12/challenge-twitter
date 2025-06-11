using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPI.Application.Abstractions.Messaging;
using TwitterAPI.Domain.Abstractions;
using TwitterAPI.Domain.Entities.Tweet;
using TwitterAPI.Interfaces.Responses;

namespace Application.Features.Tweets.CommandCreateTweet
{
    public class CreateTweetCommandHandler : ICommandHandler<CreateTweetCommand>
    {
        private readonly ITweetRepository _tweetRepository;

        public CreateTweetCommandHandler(ITweetRepository tweetRepository)
        {
            this._tweetRepository = tweetRepository;
        }

        public async Task<IResponse> Handle(CreateTweetCommand request, CancellationToken cancellationToken)
        {
            var tweet = new Tweet
            {
                Content = request.dto.Content,
                UserId = request.dto.UserId,
            };

            await _tweetRepository.AddAsync(tweet, cancellationToken);

            await this._tweetRepository.SaveAsync(cancellationToken);

            return Response.Success(tweet);
        }
    }
}
