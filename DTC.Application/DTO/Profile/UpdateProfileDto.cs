namespace DTC.Application.DTO.Profile
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
    }
}
