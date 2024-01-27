using AkakceProject.Application.Mediators;
using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AkakceProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var users = await _mediator.Send(new UserMediator.GetAllUsersQuery());
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUsers query: {stopwatch.Elapsed}");
                return Ok(users.Take(200));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUsers query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserById")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var user = await _mediator.Send(new UserMediator.GetUserQuery { UserId = id });
                if (user == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for GetUser query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUser query: {stopwatch.Elapsed}");
                return Ok(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUser query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserInfo")]
        public async Task<ActionResult<UserResponse>> GetUserInfo(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var user = await _mediator.Send(new UserMediator.GetUserInfoQuery { UserId = id });
                if (user == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for GetUserInfo query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUserInfo query: {stopwatch.Elapsed}");
                return Ok(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUserInfo query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<ActionResult<User>> CreateUser(UserForCreationDto user)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var createdUser = await _mediator.Send(new UserMediator.CreateUserQuery { User = user });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for CreateUser query: {stopwatch.Elapsed}");
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for CreateUser query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
        public async Task<ActionResult> UpdateUser(int id, UserForUpdateDto user)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var dbUser = await _mediator.Send(new UserMediator.GetUserQuery { UserId = id });
                if (dbUser == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for UpdateUser query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                await _mediator.Send(new UserMediator.UpdateUserQuery { UserId = id, User = user });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for UpdateUser query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for UpdateUser query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("DeactivateUser")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var dbUser = await _mediator.Send(new UserMediator.GetUserQuery { UserId = id });
                if (dbUser == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for DeactivateUser query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                await _mediator.Send(new UserMediator.DeactivateUserQuery { UserId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeactivateUser query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeactivateUser query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
