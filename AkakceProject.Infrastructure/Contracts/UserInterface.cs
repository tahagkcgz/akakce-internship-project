using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;

namespace AkakceProject.Infrastructure.Contracts
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetUsers();
        public Task<User> GetUser(int id);
        public Task<bool?> IsUserAdmin(int id);
        public Task<UserResponse> GetUserInfo(int id);
        public Task<User> CreateUser(UserForCreationDto user);
        public Task UpdateUser(int id, UserForUpdateDto user);
        public Task DeactivateUser(int id);
    }
}
