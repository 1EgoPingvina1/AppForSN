namespace DTC.Domain.Entities.Main
{
    public class AuthorGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        public DateTime RegDate { get; set; } = DateTime.UtcNow;
        public int RegUser_ID { get; set; }
        public virtual ICollection<AuthorGroupMember> Members { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}