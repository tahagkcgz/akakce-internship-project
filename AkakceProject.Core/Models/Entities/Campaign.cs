using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AkakceProject.Core.Entities
{
    public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }
        [Required]
        public string CampaignTitle { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string? StartsAt { get; set; }
        public string? EndsAt { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
    }
}
