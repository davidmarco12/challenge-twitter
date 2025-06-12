using Application.DTOs;
using TwitterAPI.Application.Abstractions.Messaging;

namespace Application.Features.Follow.CommandFollowUser
{
    public record FollowUserCommand(FollowUserDTO dto) : ICommand
    {
    }
}
