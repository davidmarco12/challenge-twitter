namespace TwitterAPI.Domain.Abstractions
{
    public record Error(string code, string description)
    {
        public static readonly Error FollowError = new("FOLLOW_ERROR", "Same user");

        public static readonly Error InvalidAPIResponse = new("GENERIC_ERROR.Invalid_API_Response", "Invalid API Response");

        public static Error Custom(string code, string description) => new(code, description);
    }
}
