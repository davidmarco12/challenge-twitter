using Application.Dtos;
using System;
using TwitterAPI.Application.Abstractions.Messaging;

namespace Application.Features.Tweets.QueryGetTimeline
{
    public record GetTimelineQuery(GetTimelineDTO dto) : IQueryPaginated<TimelineDTO>;
}
