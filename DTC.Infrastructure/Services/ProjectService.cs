using AutoMapper;
using DTC.Application.DTO;
using DTC.Application.DTO.Project;
using DTC.Application.Interfaces;
using DTC.Application.Interfaces.RabbitMQ;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Identity;
using DTC.Domain.Entities.Main;
using System.Security.Claims;

namespace DTC.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IMinioFileService _minioStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRabbitMqPublisher _rabbitMqService;
        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IRabbitMqPublisher rabbitMqService, IMinioFileService minioStorage)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;
            _minioStorage = minioStorage;
        }

        public async Task<ProjectResponseDto> CreateAsync(CreateProjectDTO createDto,ClaimsPrincipal user)
        {
            var project = _mapper.Map<Project>(createDto);

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            project.CreaterId = int.Parse(userId);
            project.CreatedAt = DateTime.UtcNow;
            project.VersionDate = DateTime.UtcNow;
            project.StatusId = 1;

            if (createDto.PhotoFile != null)
            {
                using var stream = createDto.PhotoFile.OpenReadStream();
                string objectName = $"{Guid.NewGuid()}_{createDto.PhotoFile.FileName}";
                string url = await _minioStorage.UploadFileAsync(stream, objectName, createDto.PhotoFile.ContentType, "projects");
                project.PhotoUrl = url;
            }

            _unitOfWork.ProjectRepository.Add(project);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProjectResponseDto>(project);
        }

        public async Task<ProjectResponseDto?> GetByIdAsync(int id)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null) return null;

            return _mapper.Map<ProjectResponseDto>(project);
        }

        public async Task UpdateAsync(int id, UpdateProjectDTO updateDto)
        {
            var projectEntity = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (projectEntity == null)
            {
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }

            _mapper.Map(updateDto, projectEntity);
            _unitOfWork.ProjectRepository.Update(projectEntity);
        }

        public async Task DeleteAsync(int projectId)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null) return;
            _unitOfWork.ProjectRepository.DeleteByIdAsync(project);
        }

        public async Task SubmitForReviewAsync(int id)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }
            if (project.StatusId != 1)
            {
                throw new InvalidOperationException("Only registered projects can be submitted for review.");
            }
            project.StatusId = 2;
            _rabbitMqService.Publish(new { ProjectId = id, SubmittedAt = DateTime.UtcNow }, "project-review-queue");
        }

        public async Task<IEnumerable<ProjectType>> GetProjectTypesAsync() => await _unitOfWork.ProjectRepository.GetProjectTypeAsync();

       
    }
}
