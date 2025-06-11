using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPI.Application.Abstractions.Messaging;

namespace Application.Features.Follow.CommandFollowUser
{
    public record FollowUserCommand(FollowUserDTO dto) : ICommand
    {
    }

    public class FollowUserDTO
    {
        public int userId { get; set; }
        public int followerId { get; set; }
    }
}
