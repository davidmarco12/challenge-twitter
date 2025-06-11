namespace TwitterAPI.Domain.Abstractions
{
    using TwitterAPI.Interfaces.Responses;

    public class Response : IResponse
    {
        public Response()
        {
        }

        protected Response(bool isSuccess, List<Error?>? error = null)
        {
            if (isSuccess && error is not null)
            {
                throw new InvalidOperationException();
            }

            if (!isSuccess && error is null)
            {
                throw new InvalidOperationException();
            }

            this.IsSuccess = isSuccess;
            this.Errors = error;
        }

        public bool IsSuccess { get; set; }

        public ICollection<Error?>? Errors { get; set; }

        public static Response Success() => new (true);

        public static Response<T> Success<T>(T? value) => new (value, true);

        public static Response<T> Failure<T>(Error error) => new (default, false, new List<Error?> { error });

        public static Response<T> Failure<T>(List<Error?>? errors) => new (default, false, errors);

        public static Response Failure(Error error) => new (false, new List<Error?> { error });

        public static Response Failure(List<Error?>? errors) => new (false, errors);

        public void SetFailure(Error error)
        {
            this.Errors = new List<Error?> { error };
            this.IsSuccess = false;
        }

        public void SetFailure(List<Error?> errors)
        {
            this.Errors = errors;
            this.IsSuccess = false;
        }

        public void SetSuccess()
        {
            this.IsSuccess = true;
        }

        public bool HasError()
        {
            return !this.IsSuccess && this.Errors is not null;
        }
    }

    public class Response<T> : Response, IResponse<T>
    {
        public Response()
            : base()
        {
        }

        protected internal Response(T? data, bool isSuccess, List<Error?>? error = null)
            : base(isSuccess, error)
        {
            this.Data = data;
        }

        public T? Data { get; set; }
    }
}
