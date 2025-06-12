using Application.DTOs;
using Application.Features.Follow.CommandFollowUser;
using Application.Features.Users.GetUsersQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TwitterAPI.Domain.Abstractions;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersDTO dto, CancellationToken cancellationToken)
        {
            var query = new GetUsersQuery(dto);

            var result = await this._mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("follow")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FollowUser([FromBody] FollowUserDTO dto, CancellationToken cancellationToken)
        {
            var command = new FollowUserCommand(dto);

            var result = await this._mediator.Send(command, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.BadRequest(result);
        }
    }
}
