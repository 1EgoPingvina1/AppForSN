using AutoMapper;
using DTC.Application.DTO;
using DTC.Application.DTO.Project;
using DTC.Application.Interfaces;
using DTC.Application.Interfaces.RabbitMQ;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Main;

namespace DTC.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRabbitMqPublisher _rabbitMqService;
        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper,IRabbitMqPublisher rabbitMqService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;

        }

        public async Task<ProjectResponseDto> CreateAsync(CreateProjectDTO createDto)
        {
            var project = _mapper.Map<Project>(createDto);

            project.CreatedAt = DateTime.UtcNow;
            project.VersionDate = DateTime.UtcNow;
            project.StatusId = 1; 

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
