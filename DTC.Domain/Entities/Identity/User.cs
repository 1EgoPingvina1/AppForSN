using Microsoft.AspNetCore.Identity;

namespace DTC.Domain.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public bool IsAuthor { get; set; } = false;
        public byte[]? Avatar { get; set; }
        public string? Description { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? BlockDate { get; set; }
        public DateTime? BanDate { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } 
        public ICollection<AppUserRole> UserRoles { get; set; } 
    }
}
