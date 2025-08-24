using System.ComponentModel.DataAnnotations;

namespace DTC.Application.DTO
{
    public class CreateAuthorGroupDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public string? Photo {  get; set; }
    }
}
