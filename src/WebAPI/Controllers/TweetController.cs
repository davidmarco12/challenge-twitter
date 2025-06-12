using Application.Dtos;
using Application.DTOs;
using Application.Features.Tweets.CommandCreateTweet;
using Application.Features.Tweets.QueryGetTimeline;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/tweet")]
    public class TweetController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TweetController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTweets([FromQuery] GetTimelineDTO dto, CancellationToken cancellationToken) 
        {
            var query = new GetTimelineQuery(dto);
            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result) : this.NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTweets(CreateTweetDTO dto)
        {
            var command = new CreateTweetCommand(dto);

            var result = await this._mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
