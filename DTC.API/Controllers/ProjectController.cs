using DTC.API.Helpers;
using DTC.Application.DTO.Project;
using DTC.Application.Interfaces.Repo;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IWebHostEnvironment _env;

        public ProjectController(IProjectService projectService, IWebHostEnvironment env)
        {
            _projectService = projectService;
            _env = env;
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
                return NoContent(); // Успешно, тело ответа не требуется
            }
            catch (KeyNotFoundException ex)
            {
                // Проект не найден
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Проект не в том статусе для отправки на ревью (бизнес-правило)
                return BadRequest(ex.Message);
            }
        }
    }
}
