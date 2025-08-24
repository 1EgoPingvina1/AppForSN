using DTC.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTC.Domain.Entities.Main
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        public string? Description { get; set; }

        public DateTime RegDate { get; set; }

        // --- Связи ---


        public int UserId { get; set; }


        public virtual User User { get; set; }


        public virtual ICollection<AuthorGroupMember> GroupMemberships { get; set; } = new List<AuthorGroupMember>();
    }
}
