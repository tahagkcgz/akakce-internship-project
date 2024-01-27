using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using Dapper;

namespace AkakceProject.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDapperContext _context;

        public UserRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var query = "SELECT * FROM AkakceUser WITH (NOLOCK)";
            return await _context.QueryAsync<User>(query);
        }

        public async Task<User> GetUser(int id)
        {
            var query = "SELECT * FROM AkakceUser WITH (NOLOCK) WHERE UserId = @UserId";
            return await _context.QuerySingleOrDefaultAsync<User>(query, new { UserId = id });
        }

        public async Task<bool?> IsUserAdmin(int id)
        {
            var query = "SELECT IsAdmin FROM AkakceUser WITH (NOLOCK) WHERE UserId = @UserId";
            return await _context.QuerySingleOrDefaultAsync<bool?>(query, new { UserId = id });
        }

        public async Task<UserResponse> GetUserInfo(int id)
        {
            var isAdmin = await IsUserAdmin(id);
            if (!isAdmin.HasValue) return null;

            var userDict = new Dictionary<string, UserResponse>();

            var query = isAdmin.Value ? "SELECT U.UserName AS AuthUserName, C.CampaignTitle, C.IsActive, C.StartsAt, C.EndsAt, " +
                                        "U.UserName AS BrandName, C.CampaignTitle AS CampaignName, P.ProductName, P.ProductDescription, P.ProductPrice, " +
                                        "U.UserName, U.EmailAddress, U.PhoneNumber FROM AkakceUser U, AkakceCampaign C WITH (NOLOCK) " +
                                        "RIGHT JOIN AkakceProduct P ON C.CampaignId = P.CampaignId " +
                                        "WHERE U.UserId = " + id + " AND P.UserId = " + id
                                        :
                                        "SELECT Brand.UserName AS AuthUserName, C.CampaignTitle, C.IsActive, C.StartsAt, C.EndsAt, " +
                                        "Brand.UserName AS BrandName, C.CampaignTitle AS CampaignName, P.ProductName, P.ProductDescription, P.ProductPrice, " +
                                        "U.UserName, U.EmailAddress, U.PhoneNumber FROM AkakceUser U, AkakceCampaign C WITH (NOLOCK) " +
                                        "RIGHT JOIN dbo.AkakceProduct P ON C.CampaignId = P.CampaignId " +
                                        "LEFT JOIN dbo.AkakceUser AS Brand ON Brand.UserId = P.UserId " +
                                        "WHERE U.UserId = " + id + " AND (ABS(CAST((BINARY_CHECKSUM(*) * RAND()) as int)) % 100) < 2";

            var userReturn = await _context.QueryAsync<CampaignResponse, ProductResponse, UserResponse, UserResponse>(query,
                (campaign, product, user) => {
                    if (!userDict.TryGetValue(user.UserName, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.UserCampaigns = new List<CampaignResponse>();
                        userEntry.UserProducts = new List<ProductResponse>();
                        userDict[user.UserName] = userEntry;
                    }

                    if (campaign != null && campaign.CampaignTitle != null && !userEntry.UserCampaigns.Contains(campaign))
                    {
                        userEntry.UserCampaigns.Add(campaign);
                    }

                    if (product != null && product.ProductName != null && !userEntry.UserProducts.Contains(product))
                    {
                        userEntry.UserProducts.Add(product);
                    }

                    return userEntry;
                },
                splitOn: "BrandName,UserName"
            );

            return userReturn.First();
        }

        public async Task<User> CreateUser(UserForCreationDto user)
        {
            string query = "INSERT INTO AkakceUser (UserName, UserPassword, EmailAddress, PhoneNumber, IsAdmin) " +
                           "VALUES(@UserName, @UserPassword, @EmailAddress, @PhoneNumber, @IsAdmin)" +
                           "SELECT CAST(SCOPE_IDENTITY() as int)";

            var id = await _context.QuerySingleOrDefaultAsync<int>(query, user);

            var createdUser = new User
            {
                UserId = id,
                UserName = user.UserName,
                UserPassword = user.UserPassword,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                IsAdmin = user.IsAdmin,
                IsUserActive = true,
            };

            return createdUser;
        }

        public async Task UpdateUser(int id, UserForUpdateDto user)
        {
            string query = "UPDATE AkakceUser WITH (ROWLOCK) SET UserName = @UserName, UserPassword = @UserPassword, EmailAddress = @EmailAddress, PhoneNumber = @PhoneNumber " +
                           "WHERE UserId = @UserId";

            await _context.ExecuteAsync(query, new { user.UserName, user.UserPassword, user.EmailAddress, user.PhoneNumber, UserId = id });
        }

        public async Task DeactivateUser(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync("DELETE FROM AkakceProduct WHERE UserId = @UserId", new { UserId = id }, transaction);

                        await connection.ExecuteAsync("DELETE FROM AkakceCampaign WHERE UserId = @UserId", new { UserId = id }, transaction);

                        await connection.ExecuteAsync("UPDATE AkakceUser WITH (ROWLOCK) SET IsUserActive = 0 WHERE UserId = @UserId", new { UserId = id }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
