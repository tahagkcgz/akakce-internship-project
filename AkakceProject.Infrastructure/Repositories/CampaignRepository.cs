using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using Dapper;

namespace AkakceProject.Infrastructure.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly IDapperContext _context;

        public CampaignRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Campaign>> GetCampaigns()
        {
            var query = "SELECT * FROM AkakceCampaign WITH (NOLOCK)";
            return await _context.QueryAsync<Campaign>(query);
        }

        public async Task<Campaign> GetCampaign(int id)
        {
            var query = "SELECT * FROM AkakceCampaign WITH (NOLOCK) WHERE CampaignId = @CampaignId";
            return await _context.QuerySingleOrDefaultAsync<Campaign>(query, new { CampaignId = id });
        }

        public async Task<IEnumerable<Campaign>> GetUserCampaigns(int id)
        {
            var query = "SELECT * FROM AkakceCampaign WITH (NOLOCK) WHERE UserId = @UserId";
            return await _context.QueryAsync<Campaign>(query, new { UserId = id });
        }

        public async Task<CampaignResponse> GetCampaignInfo(int userId, int campaignId)
        {
            var userRepo = new UserRepository(_context);
            var isAdmin = await userRepo.IsUserAdmin(userId);
            if (!isAdmin.HasValue) return null;

            var query = "SELECT U.UserName AS AuthUserName, C.CampaignTitle, C.IsActive, " +
                        "C.StartsAt, C.EndsAt " +
                        "FROM AkakceUser U WITH (NOLOCK) " +
                        "LEFT JOIN AkakceCampaign C ON U.UserId = C.UserId " +
                        "WHERE C.CampaignId = " + campaignId;

            query += isAdmin.Value ? " AND U.UserId = " + userId : "";

            return await _context.QuerySingleOrDefaultAsync<CampaignResponse>(query);
        }

        public async Task<Campaign> CreateCampaign(CampaignForCreationDto campaign)
        {
            string query = "INSERT INTO AkakceCampaign (CampaignTitle, IsActive, StartsAt, EndsAt, UserId) " +
                           "VALUES(@CampaignTitle, @IsActive, @StartsAt, @EndsAt, @UserId)\n" +
                           "SELECT @@IDENTITY";

            var id = await _context.QuerySingleOrDefaultAsync<int>(query, campaign);

            var createdCampaign = new Campaign
            {
                CampaignId = id,
                CampaignTitle = campaign.CampaignTitle,
                IsActive = campaign.IsActive,
                StartsAt = campaign.StartsAt,
                EndsAt = campaign.EndsAt,
                UserId = campaign.UserId,
            };

            return createdCampaign;
        }

        public async Task UpdateCampaign(int id, CampaignForUpdateDto campaign)
        {
            var query = "UPDATE AkakceCampaign WITH (ROWLOCK) SET IsActive = @IsActive, StartsAt = @StartsAt, EndsAt = @EndsAt " +
                        "WHERE CampaignId = @CampaignId";

            await _context.ExecuteAsync(query, new { campaign.IsActive, campaign.StartsAt, campaign.EndsAt, CampaignId = id });
        }

        public async Task DeleteCampaign(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync("UPDATE AkakceProduct WITH (ROWLOCK) SET CampaignId = NULL WHERE CampaignId = @CampaignId", new { CampaignId = id }, transaction);

                        await connection.ExecuteAsync("DELETE FROM AkakceCampaign WHERE CampaignId = @CampaignId", new { CampaignId = id }, transaction);

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

        public async Task DeleteUserCampaigns(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync("UPDATE AkakceProduct WITH (ROWLOCK) SET CampaignId = NULL WHERE UserId = @UserId", new { UserId = id }, transaction);

                        await connection.ExecuteAsync("DELETE FROM AkakceCampaign WHERE UserId = @UserId", new { UserId = id }, transaction);

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
