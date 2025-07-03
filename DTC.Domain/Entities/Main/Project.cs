using DTC.Domain.Entities.Identity;

namespace DTC.Domain.Entities.Main
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime VersionDate { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProjectFiles { get; set; } // путь до архива или id

        public int CreaterId { get; set; }
        public User Creater { get; set; }

        public int StatusId { get; set; }
        public ProjectStatus Status { get; set; }

        public int ProjectTypeId { get; set; }
        public ProjectType Type { get; set; }

        public int AuthorGroupId { get; set; }
        public AuthorGroup Group { get; set; }
    }
}
