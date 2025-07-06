using System;

namespace AppForSNForUsers.DTOs
{
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public bool IsAuthor { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
