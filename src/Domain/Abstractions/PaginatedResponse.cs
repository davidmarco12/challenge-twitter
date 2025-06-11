namespace TwitterAPI.Domain.Abstractions
{
    using TwitterAPI.Interfaces.Responses;
    using TwitterAPI.Responses;

    public class PaginatedResponse<T> : Response, IPaginatedResponse<T>
    {
        public PaginatedResponse()
            : base()
        {
            this.PaginationData = new PaginationData();
        }

        protected PaginatedResponse(ICollection<T?>? data, PaginationData painationData, bool isSuccess, List<Error?>? error = null)
            : base(isSuccess, error)
        {
            this.Data = data;
            this.PaginationData = painationData;
        }

        public PaginationData PaginationData { get; set; }

        public ICollection<T?>? Data { get; set; }

        public static PaginatedResponse<T> Success(ICollection<T?>? value, PaginationData paginationData) => new (value, paginationData, true);

        public static new PaginatedResponse<T> Failure(Error error) => new (default, new PaginationData(), false, new List<Error?> { error });

        public static new PaginatedResponse<T> Failure(List<Error?> errors) => new (default, new PaginationData(), false, errors);
    }
}
