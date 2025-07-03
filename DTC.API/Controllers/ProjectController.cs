using DTC.API.Helpers;
using DTC.Application.DTO;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectService;
        private readonly IWebHostEnvironment _env;

        public ProjectController(IProjectRepository projectService, IWebHostEnvironment webHostEnvironment)
        {
            _projectService = projectService;
            _env = webHostEnvironment;
        }

        [Authorize(Roles = "2")] // Только авторы
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateProjectDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var photoPath = dto.Photo != null
                ? await FileHandlers.SaveFileAsync(dto.Photo, "uploads/projects", _env.WebRootPath)
                : null;

            var archivePath = dto.ProjectFiles != null
                ? await FileHandlers.SaveFileAsync(dto.ProjectFiles, "uploads/archives", _env.WebRootPath)
                : null;

            var project = new Project
            {
                Name = dto.Name,
                Version = dto.Version,
                VersionDate = DateTime.UtcNow,
                Description = dto.Description,
                ProjectTypeId = dto.ProjectType_ID,
                AuthorGroupId = dto.AuthorGroup_ID,
                StatusId = 1,
                CreatedAt = DateTime.UtcNow,
                CreaterId = userId,
                PhotoUrl = photoPath,
                ProjectFiles = archivePath
            };

            var result = await _projectService.CreateProjectAsync(project);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("my-projects")]
        public async Task<IActionResult> GetMyProjects()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var projects = await _projectService.GetProjectsByUserAsync(userId);
            return Ok(projects);
        }
    }

}
