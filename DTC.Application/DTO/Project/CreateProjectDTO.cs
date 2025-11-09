using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DTC.Application.DTO.Project
{
    public class CreateProjectDTO
    {
        public string Name { get; set; } = null!;
        public string Version { get; set; } = null!;
        public DateTime VersionDate { get; set; }
        public string Description { get; set; } = null!;
        public bool IsOpenSource { get; set; }
        public string? PhotoUrl { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public int AuthorGroupId { get; set; }
        public int StatusId { get; set; }
        public int ProjectTypeId { get; set; }
        public int BeginAge { get; set; }
        public int EndAge { get; set; }
        public List<IFormFile> Files { get; set; } = new();
    }
}
