using Microsoft.AspNetCore.Http;

namespace DTC.Application.DTO
{
    public class CreateProjectDTO
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public int ProjectType_ID { get; set; }
        public int AuthorGroup_ID { get; set; }
        public IFormFile Photo { get; set; }
        public IFormFile ProjectFiles { get; set; }
    }
}
