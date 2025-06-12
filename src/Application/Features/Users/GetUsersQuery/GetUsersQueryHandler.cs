using Application.DTOs;
using TwitterAPI.Application.Abstractions.Messaging;
using TwitterAPI.Domain.Abstractions;
using TwitterAPI.Domain.Entities;
using TwitterAPI.Interfaces.Responses;
using TwitterAPI.Responses;

namespace Application.Features.Users.GetUsersQuery
{
    public class GetUsersQueryHandler : IQueryPaginatedHandler<GetUsersQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<IPaginatedResponse<UserDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var (users, paginationData) = await this._userRepository.GetPaginatedUsers(request.dto.PageSize, request.dto.PageNumber);

            var userListDTO = users?.Select(user => new UserDTO
            {
                Id = user.Id,
                username = user.UserName
            }).ToList() ?? new List<UserDTO>();

            return PaginatedResponse<UserDTO>.Success(userListDTO!, paginationData);
        }
    }
}
