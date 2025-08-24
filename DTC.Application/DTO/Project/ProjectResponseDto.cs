using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTC.Application.DTO.Project
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime VersionDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Guid AuthorGroupId { get; set; }
    }
}
