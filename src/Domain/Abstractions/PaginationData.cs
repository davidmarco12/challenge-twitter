namespace TwitterAPI.Responses
{
    using TwitterAPI.Interfaces.Responses;

    public class PaginationData : IPaginationData
    {
        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}
