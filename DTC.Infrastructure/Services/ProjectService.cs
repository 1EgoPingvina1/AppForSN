using AutoMapper;
using DTC.Application.DTO.Project;
using DTC.Application.ErrorHandlers;
using DTC.Application.Interfaces;
using DTC.Application.Interfaces.RabbitMQ;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Main;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DTC.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IMinioFileService _minioStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRabbitMqPublisher _rabbitMqService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IRabbitMqPublisher rabbitMqService, IMinioFileService minioStorage, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;
            _minioStorage = minioStorage;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProjectResponseDto> CreateAsync(CreateProjectDTO createDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var project = _mapper.Map<Project>(createDto);

                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null)
                    throw new HttpExeption(401, "Токен не действителен");

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value ?? throw new HttpExeption(401, "Unauthirized");
                project.CreaterId = int.Parse(userId);
                project.CreatedAt = DateTime.UtcNow;
                project.VersionDate = DateTime.UtcNow;
                project.StatusId = 1;

                if (createDto.PhotoFile != null)
                {
                    using var stream = createDto.PhotoFile.OpenReadStream();
                    string objectName = $"{Guid.NewGuid()}_{createDto.PhotoFile.FileName}";
                    string url = await _minioStorage.UploadFileAsync(stream, objectName, createDto.PhotoFile.ContentType, "project-photos");
                    project.PhotoUrl = url;
                }

                if (createDto.Files != null && createDto.Files.Any())
                {
                    var uploadedFiles = await _minioStorage.UploadProjectFilesAsync(
                        project.Id,
                        createDto.Files.ToList(),
                        "project-files",
                        isMainFile: false);

                    foreach (var file in uploadedFiles)
                    {
                        project.Files ??= new List<ProjectFile>();
                        project.Files.Add(file);
                    }
                }

                _unitOfWork.ProjectRepository.Add(project);

                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<ProjectResponseDto>(project);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw new HttpExeption(500, "Transaction has been canceled");
            }
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
