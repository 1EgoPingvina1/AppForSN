﻿using Microsoft.AspNetCore.Identity;

namespace DTC.Domain.Entities.Identity
{
    public class UserRoles : IdentityUserRole<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
