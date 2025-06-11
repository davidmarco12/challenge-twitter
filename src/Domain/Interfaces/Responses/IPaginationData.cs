namespace TwitterAPI.Interfaces.Responses
{
    public interface IPaginationData
    {
        int TotalCount { get; set; }

        int PageNumber { get; set; }

        int PageSize { get; set; }

        int TotalPages { get; set; }
    }
}
