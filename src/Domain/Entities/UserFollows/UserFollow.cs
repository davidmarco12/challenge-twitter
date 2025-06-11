using TwitterAPI.Domain.Abstractions;

namespace TwitterAPI.Domain.Entities
{
    public sealed class UserFollow : Entity
    {
        required public int FollowerId { get; set; }

        public User? Follower { get; set; }


        required public int FollowingId { get; set; }

        public User? Following { get; set; }

    }
}
