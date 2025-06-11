namespace TwitterAPI.Interfaces.Responses
{
    using TwitterAPI.Domain.Abstractions;
    using TwitterAPI.Responses;

    public interface IResponse
    {
        bool IsSuccess { get; set; }

        ICollection<Error?>? Errors { get; set; }
    }

    public interface IResponse<T> : IResponse
    {
        T? Data { get; set; }
    }

    public interface IPaginatedResponse<T> : IResponse
    {
        ICollection<T?>? Data { get; set; }

        PaginationData PaginationData { get; set; }
    }
}
