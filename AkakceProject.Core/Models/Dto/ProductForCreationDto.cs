namespace AkakceProject.Core.Dto
{
    public class ProductForCreationDto
    {
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int ProductPrice { get; set; }
        public int UserId { get; set; }
        public int? CampaignId { get; set; }
    }
}
