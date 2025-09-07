using AutoMapper;
using DTC.Application.DTO.Project;
using DTC.Application.Interfaces.RabbitMQ;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Main;
using DTC.Infrastructure.Services;
using Moq;

namespace DTC.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRabbitMqPublisher> _mockRabbitMqPublisher;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockRabbitMqPublisher = new Mock<IRabbitMqPublisher>();

            _projectService = new ProjectService(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockRabbitMqPublisher.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddProjectAndReturnDto()
        {
            var createDto = new CreateProjectDTO
            {
                Name = "Test Project",
                CreaterId = 1,
                StatusId = 1,
                Version = "5",
                Description = "A project for testing",
                ProjectTypeId = 1,
                AuthorGroupId = 4
            };
            var projectEntity = new Project { Id = 1, Name = "Test Project" };
            var projectResponseDto = new ProjectResponseDto { Id = 1, Name = "Test Project" };

            _mockMapper.Setup(m => m.Map<Project>(createDto)).Returns(projectEntity);
            _mockMapper.Setup(m => m.Map<ProjectResponseDto>(projectEntity)).Returns(projectResponseDto);

            var result = await _projectService.CreateAsync(createDto);

            // Assert
            _mockUnitOfWork.Verify(u => u.ProjectRepository.Add(It.Is<Project>(p =>
                p.Name == createDto.Name &&
                p.CreatedAt.Kind == DateTimeKind.Utc &&
                p.VersionDate.Kind == DateTimeKind.Utc && 
                p.StatusId == 1
            )), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(projectResponseDto.Id, result.Id);
            Assert.Equal(projectResponseDto.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProjectDto_WhenProjectExists()
        {
            // Arrange
            var projectId = 1;
            var projectEntity = new Project { Id = projectId, Name = "Existing Project" };
            var projectResponseDto = new ProjectResponseDto { Id = projectId, Name = "Existing Project" };

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync(projectEntity);
            _mockMapper.Setup(m => m.Map<ProjectResponseDto>(projectEntity)).Returns(projectResponseDto);

            // Act
            var result = await _projectService.GetByIdAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.Id);
            Assert.Equal(projectResponseDto.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 99;

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            // Act
            var result = await _projectService.GetByIdAsync(projectId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProject_WhenProjectExists()
        {
            // Arrange
            var projectId = 1;
            var updateDto = new UpdateProjectDTO { Name = "Updated Project Name", Description = "Updated description" };
            var existingProject = new Project { Id = projectId, Name = "Original Name", Description = "Original description", StatusId = 1 };

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync(existingProject);

            // Act
            await _projectService.UpdateAsync(projectId, updateDto);

            // Assert
            _mockMapper.Verify(m => m.Map(updateDto, existingProject), Times.Once);
            _mockUnitOfWork.Verify(u => u.ProjectRepository.Update(existingProject), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 99;
            var updateDto = new UpdateProjectDTO { Name = "Non Existent Project" };

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _projectService.UpdateAsync(projectId, updateDto));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProject_WhenProjectExists()
        {
            // Arrange
            var projectId = 1;
            var projectEntity = new Project { Id = projectId, Name = "Project to Delete" };

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync(projectEntity);

            // Act
            await _projectService.DeleteAsync(projectId);

            // Assert
            _mockUnitOfWork.Verify(u => u.ProjectRepository.DeleteByIdAsync(projectEntity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDoNothing_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 99;

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            // Act
            await _projectService.DeleteAsync(projectId);

            // Assert
            _mockUnitOfWork.Verify(u => u.ProjectRepository.DeleteByIdAsync(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public async Task SubmitForReviewAsync_ShouldUpdateStatusAndPublishMessage_WhenProjectIsRegistered()
        {
            // Arrange
            var projectId = 1;
            var projectEntity = new Project { Id = projectId, Name = "Reviewable Project", StatusId = 1 }; // StatusId = 1 means Registered

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync(projectEntity);

            // Act
            await _projectService.SubmitForReviewAsync(projectId);

            // Assert
            Assert.Equal(2, projectEntity.StatusId); // StatusId should be updated to 2 (Submitted for review)
            _mockRabbitMqPublisher.Verify(r => r.Publish(
                It.Is<object>(msg => (int)msg.GetType().GetProperty("ProjectId").GetValue(msg) == projectId),
                "project-review-queue"), Times.Once);
        }

        [Fact]
        public async Task SubmitForReviewAsync_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 99;

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _projectService.SubmitForReviewAsync(projectId));
        }

        [Fact]
        public async Task SubmitForReviewAsync_ShouldThrowInvalidOperationException_WhenProjectIsNotRegistered()
        {
            // Arrange
            var projectId = 1;
            var projectEntity = new Project { Id = projectId, Name = "Already Reviewed Project", StatusId = 2 }; // StatusId = 2 means Submitted

            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync(projectEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _projectService.SubmitForReviewAsync(projectId));
            // Ensure status was not changed and no message was published
            Assert.Equal(2, projectEntity.StatusId);
            _mockRabbitMqPublisher.Verify(r => r.Publish(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }
    }
}

