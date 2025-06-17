namespace DTC.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime VersionDate { get; set; }
        public string Description { get; set; }
        public Guid StatusId { get; set; }
        public ProjectStatus Status { get; set; }

        public Guid ProjectTypeId { get; set; }
        public ProjectType Type { get; set; }

        public Guid AuthorGroupId { get; set; }
        public AuthorGroup Group { get; set; }
    }
}
