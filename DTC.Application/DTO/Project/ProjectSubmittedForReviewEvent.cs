using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTC.Application.DTO.Project
{
    public class ProjectSubmittedForReviewEvent
    {
        public int ProjectId { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
