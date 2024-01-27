using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AkakceProject.Core.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        [Required]
        public int ProductPrice { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Campaign")]
        public int? CampaignId { get; set; }
    }
}
