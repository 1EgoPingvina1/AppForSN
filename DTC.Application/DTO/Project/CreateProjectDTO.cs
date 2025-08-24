using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DTC.Application.DTO.Project
{
    public class CreateProjectDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string Version { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        public int ProjectType_ID { get; set; }
        public int AuthorGroup_ID { get; set; }
        public IFormFile Photo { get; set; }
        public IFormFile ProjectFiles { get; set; }
    }
}
