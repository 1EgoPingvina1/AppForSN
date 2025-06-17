
using Microsoft.AspNetCore.Identity;

namespace DTC.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
