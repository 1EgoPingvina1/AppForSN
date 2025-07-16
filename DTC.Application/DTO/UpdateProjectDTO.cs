using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTC.Application.DTO
{
    public class UpdateProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Version { get; set; }
        public int ProjectTypeId { get; set; }
        public int AuthorGroupId { get; set; }
    }
}
