namespace AkakceProject.Core.Responses
{
    public class CampaignResponse
    {
        public string AuthUserName { get; set; }
        public string CampaignTitle { get; set; }
        public bool IsActive { get; set; }
        public string? StartsAt { get; set; }
        public string? EndsAt { get; set; }
    }
}
