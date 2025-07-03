using DTC.Domain.Entities.Identity;

namespace DTC.Domain.Entities.Main
{
    public class AuthorGroupMember
    {
        public int AuthorGroup_ID { get; set; }
        public int Author_ID { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public int RegUser_ID { get; set; }

        public AuthorGroup AuthorGroup { get; set; }
        public User Author { get; set; }
    }
}