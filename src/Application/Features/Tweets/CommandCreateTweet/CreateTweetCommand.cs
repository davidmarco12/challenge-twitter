using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPI.Application.Abstractions.Messaging;

namespace Application.Features.Tweets.CommandCreateTweet
{
    public record CreateTweetCommand(CreateTweetDTO dto) : ICommand;
}
