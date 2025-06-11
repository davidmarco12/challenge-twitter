namespace TwitterAPI.Interfaces.Requests
{
    public interface IPaginatedRequest
    {
        int PageNumber { get; set; }

        int PageSize { get; set; }
    }
}
