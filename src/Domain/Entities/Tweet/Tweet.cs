using TwitterAPI.Domain.Abstractions;

namespace TwitterAPI.Domain.Entities.Tweet
{
    public class Tweet : Entity
    {
        required public string Content { get; set; }

        required public int UserId { get; set; }

        public User? User { get; set; }
    }
}
