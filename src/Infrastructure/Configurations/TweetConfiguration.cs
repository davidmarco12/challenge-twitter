using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitterAPI.Domain.Entities.Tweet;

namespace Infrastructure.Configurations
{
    public class TweetConfiguration : IEntityTypeConfiguration<Tweet>
    {
        public void Configure(EntityTypeBuilder<Tweet> builder)
        {
            builder.ToTable(nameof(Tweet), "Tweet");
            builder.Property(x => x.Content).HasMaxLength(128);
        }
    }
}
