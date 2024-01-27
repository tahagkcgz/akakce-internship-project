namespace AkakceProject.Core.Responses
{
    public class UserResponse
    {
        public UserResponse()
        {
            UserCampaigns = new List<CampaignResponse>();
            UserProducts = new List<ProductResponse>();
        }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<CampaignResponse> UserCampaigns { get; set; }
        public ICollection<ProductResponse> UserProducts { get; set; }
    }
}
