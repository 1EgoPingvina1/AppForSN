using DTC.API.Helpers;
using DTC.Application.DTO;
using DTC.Application.Interfaces;
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
        private readonly IProjectRepository _projectService;
        private readonly IWebHostEnvironment _env;

        public ProjectController(IProjectRepository projectService, IWebHostEnvironment env)
        {
            _projectService = projectService;
            _env = env;
        }

        private int? GetUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : null;
        }

        [Authorize(Roles = "Author")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateProjectDTO dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid user.");

            var photoPath = dto.Photo != null
                ? await FileHandlers.SaveFileAsync(dto.Photo, "uploads/projects", _env.WebRootPath)
                : null;

            var archivePath = dto.ProjectFiles != null
                ? await FileHandlers.SaveFileAsync(dto.ProjectFiles, "uploads/archives", _env.WebRootPath)
                : null;

            var status = await _projectService.GetRegisterStatus();
            if (status == null) return BadRequest("Unable to determine initial project status.");

            var project = new Project
            {
                Name = dto.Name,
                Version = dto.Version,
                VersionDate = DateTime.UtcNow,
                Description = dto.Description,
                ProjectTypeId = dto.ProjectType_ID,
                AuthorGroupId = dto.AuthorGroup_ID,
                StatusId = status.Id,
                CreatedAt = DateTime.UtcNow,
                CreaterId = userId.Value,
                PhotoUrl = photoPath,
                ProjectFiles = archivePath
            };

            var created = await _projectService.CreateProjectAsync(project);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize]
        [HttpGet("my-projects")]
        public async Task<IActionResult> GetMyProjects()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid user.");

            var projects = await _projectService.GetProjectsByUserAsync(userId.Value);
            return Ok(projects);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var project = await _projectService.GetByIdAsync(id);
            if (project == null || project.CreaterId != userId)
                return NotFound();

            return Ok(project);
        }

        [Authorize(Roles = "Author")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDTO dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var project = await _projectService.GetByIdAsync(id);
            if (project == null || project.CreaterId != userId)
                return NotFound("Project not found or unauthorized");

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.Version = dto.Version;
            project.VersionDate = DateTime.UtcNow;
            project.ProjectTypeId = dto.ProjectTypeId;
            project.AuthorGroupId = dto.AuthorGroupId;

            await _projectService.UpdateProjectAsync(project);
            return NoContent();
        }

        [Authorize(Roles = "Author")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var project = await _projectService.GetByIdAsync(id);
            if (project == null || project.CreaterId != userId)
                return NotFound();

            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}
