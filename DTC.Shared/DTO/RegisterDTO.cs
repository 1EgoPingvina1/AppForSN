namespace DTC.API.DTO
{
    public class RegisterDTO
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public bool IsAuthor { get; set; } = false;
        public DateTime Birthday { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
