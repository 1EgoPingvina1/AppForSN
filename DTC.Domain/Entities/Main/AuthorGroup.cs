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
        public ICollection<AuthorGroupMember> Members { get; set; }
    }
}