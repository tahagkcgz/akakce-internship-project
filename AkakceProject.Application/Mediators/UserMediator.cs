using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using MediatR;

namespace AkakceProject.Application.Mediators
{
    public class UserMediator
    {
        public class GetAllUsersQuery : IRequest<IEnumerable<User>>
        {
        }

        public class GetUserQuery : IRequest<User>
        {
            public int UserId { get; set; }
        }

        public class GetUserInfoQuery : IRequest<UserResponse>
        {
            public int UserId { get; set; }
        }

        public class CreateUserQuery : IRequest<User>
        {
            public UserForCreationDto User { get; set; }
        }

        public class UpdateUserQuery : IRequest
        {
            public int UserId { get; set; }
            public UserForUpdateDto User { get; set; }
        }

        public class DeactivateUserQuery : IRequest
        {
            public int UserId { get; set; }
        }

        public class UserQueryHandler :
            IRequestHandler<GetAllUsersQuery, IEnumerable<User>>,
            IRequestHandler<GetUserQuery, User>,
            IRequestHandler<GetUserInfoQuery, UserResponse>,
            IRequestHandler<CreateUserQuery, User>,
            IRequestHandler<UpdateUserQuery>,
            IRequestHandler<DeactivateUserQuery>
        {
            private readonly IUserRepository _userRepo;

            public UserQueryHandler(IUserRepository userRepo)
            {
                _userRepo = userRepo;
            }

            public async Task<IEnumerable<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                return await _userRepo.GetUsers();
            }

            public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                return await _userRepo.GetUser(request.UserId);
            }

            public async Task<UserResponse> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
            {
                return await _userRepo.GetUserInfo(request.UserId);
            }

            public async Task<User> Handle(CreateUserQuery request, CancellationToken cancellationToken)
            {
                return await _userRepo.CreateUser(request.User);
            }

            public async Task Handle(UpdateUserQuery request, CancellationToken cancellationToken)
            {
                await _userRepo.UpdateUser(request.UserId, request.User);
            }

            public async Task Handle(DeactivateUserQuery request, CancellationToken cancellationToken)
            {
                await _userRepo.DeactivateUser(request.UserId);
            }
        }
    }
}
