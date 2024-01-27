namespace AkakceProject.Core.Dto
{
    public class CampaignForCreationDto
    {
        public string CampaignTitle { get; set; }
        public bool IsActive { get; set; }
        public string? StartsAt { get; set; }
        public string? EndsAt { get; set; }
        public int UserId { get; set; }
    }
}
