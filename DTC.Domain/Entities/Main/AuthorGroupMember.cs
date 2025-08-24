namespace DTC.Domain.Entities.Main
{
    public class AuthorGroupMember
    {
        public int Author_ID { get; set; }
        public int AuthorGroup_ID { get; set; }

        public Author Author { get; set; }
        public AuthorGroup AuthorGroup { get; set; }

        public DateTime? LeaveDate { get; set; }
        public DateTime JoinDate { get; set; }
    }
}