using DTC.Application.DTO;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Mvc;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTypeController : ControllerBase
    {
        private readonly IGenericRepository<ProjectType> _repository;

        public ProjectTypeController(IGenericRepository<ProjectType> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTypeDTO>>> GetAll()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories.Select(c => new ProjectTypeDTO
            {
                Id = c.Id,
                Name = c.Name
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectTypeDTO dto)
        {
            var category = new ProjectType { Name = dto.Name };
            await _repository.AddAsync(category);
            await _repository.SaveAsync();
            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return NotFound();

            await _repository.DeleteAsync(category.Id);
            await _repository.SaveAsync();
            return NoContent();
        }
    }
}
