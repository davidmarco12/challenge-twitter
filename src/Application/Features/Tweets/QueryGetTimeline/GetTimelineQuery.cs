using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TwitterAPI.Application.Abstractions.Messaging;
using TwitterAPI.Responses;

namespace Application.Features.Tweets.QueryGetTimeline
{
    public record GetTimelineQuery(GetTimelineDTO dto) : IQueryPaginated<TimelineDTO>;

    public class TimelineDTO
    {
        public string username { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }

    public class GetTimelineDTO
    {
        public int userId { get; set; }

        public int pageNumber { get; set; } = 1;

        public int pageSize { get; set; } = 10;
    }

    public class CachedTimelineData
    {
        [JsonPropertyName("timeline")]
        public List<TimelineDTO> Timeline { get; set; } = new();

        [JsonPropertyName("pagination")]
        public CachedPaginationData Pagination { get; set; } = new();

        public CachedTimelineData() { }

        public CachedTimelineData(List<TimelineDTO> timeline, PaginationData pagination)
        {
            Timeline = timeline;
            Pagination = new CachedPaginationData
            {
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = pagination.TotalCount,
                TotalPages = pagination.TotalPages
            };
        }
    }

    public class CachedPaginationData
    {
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        public CachedPaginationData() { }

        public PaginationData ToPaginationData()
        {
            return new PaginationData
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalCount = TotalCount,
                TotalPages = TotalPages
            };
        }
    }
}
