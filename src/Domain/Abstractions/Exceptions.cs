using TwitterAPI.Domain.Abstractions;

namespace Domain.Abstractions
{
    public sealed class CustomValidationException : Exception
    {
        public CustomValidationException(List<Error> errors)
        {
            this.Errors = errors;
        }

        public CustomValidationException(string message, Exception? innerException)
            : base(message, innerException)
        {
            this.Errors = new List<Error> { new("GENERIC_ERROR.Validation", message) };
        }

        public List<Error>? Errors { get; }
    }
}
