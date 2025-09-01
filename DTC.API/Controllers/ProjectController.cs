using DTC.Application.DTO.Project;
using DTC.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        [Authorize(Roles = "Author")]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CreateProjectDTO createDto)
        {
            var createdProject = await _projectService.CreateAsync(createDto);
            return Ok(createdProject);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Author")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDTO updateDto)
        {
            try
            {
                await _projectService.UpdateAsync(id, updateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/review")]
        [Authorize(Roles = "Author")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitForReview(int id)
        {
            try
            {
                await _projectService.SubmitForReviewAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
