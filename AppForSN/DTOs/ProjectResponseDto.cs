using System;

namespace AppForSNForUsers.DTOs
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime VersionDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int AuthorGroupId { get; set; }
    }
}
