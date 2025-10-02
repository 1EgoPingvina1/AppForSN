namespace DTC.Application.DTO.Profile
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
        public bool IsAuthor { get; set; }
    }
}
