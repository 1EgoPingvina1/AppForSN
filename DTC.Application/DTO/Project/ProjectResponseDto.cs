namespace DTC.Application.DTO.Project
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime VersionDate { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProjectFiles { get; set; }

        public int CreaterId { get; set; }
        public string CreaterName { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public int ProjectTypeId { get; set; }
        public string ProjectTypeName { get; set; }

        public int AuthorGroupId { get; set; }
        public string AuthorGroupName { get; set; }
    }
}
