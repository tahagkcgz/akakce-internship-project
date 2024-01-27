using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;

namespace AkakceProject.Infrastructure.Contracts
{
    public interface ICampaignRepository
    {
        public Task<IEnumerable<Campaign>> GetCampaigns();
        public Task<Campaign> GetCampaign(int id);
        public Task<IEnumerable<Campaign>> GetUserCampaigns(int id);
        public Task<CampaignResponse> GetCampaignInfo(int userId, int campaignId);
        public Task<Campaign> CreateCampaign(CampaignForCreationDto campaign);
        public Task UpdateCampaign(int id, CampaignForUpdateDto campaign);
        public Task DeleteCampaign(int id);
        public Task DeleteUserCampaigns(int id);
    }
}
