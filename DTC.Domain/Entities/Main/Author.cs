using DTC.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace DTC.Domain.Entities.Main
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? SecondName { get; set; }
        public string LastName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime RegDate { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<AuthorGroupMember> GroupMemberships { get; set; } = new List<AuthorGroupMember>();
    }
}
