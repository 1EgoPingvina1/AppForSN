using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DTC.Application.DTO.Project
{
    public class CreateProjectDTO
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Version { get; set; }

        public DateTime VersionDate { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string Description { get; set; }

        public string PhotoUrl { get; set; }

        [Required]
        public int CreaterId { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public int ProjectTypeId { get; set; }

        [Required]
        public int AuthorGroupId { get; set; }

        public ICollection<IFormFile> ProjectFiles { get; set; } = new List<IFormFile>();
    }
}
