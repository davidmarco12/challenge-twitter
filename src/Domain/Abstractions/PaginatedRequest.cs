using TwitterAPI.Interfaces.Requests;

namespace Domain.Abstractions
{
    public class PaginatedRequest : IPaginatedRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
