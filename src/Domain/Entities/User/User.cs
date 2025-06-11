using TwitterAPI.Domain.Abstractions;

namespace TwitterAPI.Domain.Entities
{
    public sealed class User : Entity
    {
        required public string UserName { get; set; }

        public List<UserFollow> UserFollows { get; set; } = new List<UserFollow>();
    }
}
