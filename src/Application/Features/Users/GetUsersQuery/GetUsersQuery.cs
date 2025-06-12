using Application.DTOs;
using TwitterAPI.Application.Abstractions.Messaging;

namespace Application.Features.Users.GetUsersQuery
{
    public record GetUsersQuery(GetUsersDTO dto) : IQueryPaginated<UserDTO>;
}
