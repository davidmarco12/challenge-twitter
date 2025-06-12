using Domain.Abstractions;

namespace Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string username { get; set; } = string.Empty;
    }

    public class GetUsersDTO : PaginatedRequest;

    public class FollowUserDTO
    {
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
    }
}
