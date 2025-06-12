using System.Text.Json.Serialization;
using TwitterAPI.Responses;

namespace Application.Dtos
{
    public class TimelineDTO
    {
        public int Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class GetTimelineDTO
    {
        public int UserId { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
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
