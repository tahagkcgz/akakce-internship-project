namespace AkakceProject.Core.Dto
{
    public class UserForCreationDto
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
    }
}
