using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTC.Application.DTO.Project
{
    public class UpdateProjectDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Version { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }
    }
}
