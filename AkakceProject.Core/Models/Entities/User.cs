using System.ComponentModel.DataAnnotations;

namespace AkakceProject.Core.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName{ get; set; }
        [Required]
        public string UserPassword { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public bool IsUserActive { get; set; }
    }
}
