using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPI.Application.Abstractions.Messaging;

namespace Application.Features.Tweets.CommandCreateTweet
{
    public record CreateTweetCommand(CreateTweetDTO dto) : ICommand;

    public class CreateTweetDTO
    {
        required public int UserId { get; set; }
        required public string Content { get; set; }
    }
}
