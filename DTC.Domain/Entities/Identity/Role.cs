using Microsoft.AspNetCore.Identity;

namespace DTC.Domain.Entities.Identity
{
    public class Role : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
