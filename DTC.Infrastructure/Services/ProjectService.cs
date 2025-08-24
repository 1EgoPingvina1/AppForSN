using AutoMapper;
using DTC.Application.DTO.Project;
using DTC.Application.Interfaces;
using DTC.Application.Interfaces.RabbitMQ;
using DTC.Application.Interfaces.Repo;
using DTC.Domain.Entities.Main;

namespace DTC.Infrastructure.Services
{
    public class ProjectService
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
            // 1. Маппинг DTO в сущность
            var project = _mapper.Map<Project>(createDto);

            // 2. Установка значений по умолчанию (бизнес-логика)
            project.CreatedAt = DateTime.UtcNow;
            project.VersionDate = DateTime.UtcNow;
            project.StatusId = 1; // "Зарегистрирован"

            // 3. Добавление в репозиторий и сохранение через Unit of Work
            _unitOfWork.ProjectRepository.Add(project);
            await _unitOfWork.SaveChangesAsync();

            // 4. Маппинг результата в DTO для ответа
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

            // AutoMapper обновит поля существующей сущности данными из DTO
            _mapper.Map(updateDto, projectEntity);

            // EF Core отслеживает изменения, явный вызов Update не всегда нужен, но он гарантирует установку статуса Modified
            _unitOfWork.ProjectRepository.Update(projectEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SubmitForReviewAsync(int id)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }

            // Бизнес-правило: отправить на ревью можно только проект в статусе "Зарегистрирован"
            if (project.StatusId != 1)
            {
                throw new InvalidOperationException("Only registered projects can be submitted for review.");
            }

            project.StatusId = 2; // "На проверке"
            await _unitOfWork.SaveChangesAsync();

            // Отправляем событие в RabbitMQ
            _rabbitMqService.Publish(new { ProjectId = id, SubmittedAt = DateTime.UtcNow }, "project-review-queue");
        }
    }
}
