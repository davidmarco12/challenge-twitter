using TwitterAPI.Application.Abstractions.Messaging;

namespace Application.Features.Users.GetUsersQuery
{
    public record GetUsersQuery : IQueryPaginated<UserDTO>;

    public class UserDTO
    {
        public int Id { get; set; }
        public string username { get; set; } = string.Empty;
    }
}
