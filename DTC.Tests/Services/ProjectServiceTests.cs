using AutoMapper;
using DTC.Application.DTO.Project;
using DTC.Application.ErrorHandlers;
using DTC.Application.Interfaces;
using DTC.Application.Interfaces.RabbitMQ;
using DTC.Application.Interfaces.Repo;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Main;
using DTC.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace DTC.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRabbitMqPublisher> _rabbitMqMock;
        private readonly Mock<IMinioFileService> _minioMock;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;

        private readonly Mock<IProjectRepository> _projectRepoMock;

        public ProjectServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _rabbitMqMock = new Mock<IRabbitMqPublisher>();
            _minioMock = new Mock<IMinioFileService>();
            _httpContextMock = new Mock<IHttpContextAccessor>();

            _projectRepoMock = new Mock<IProjectRepository>();
            _unitOfWorkMock.Setup(u => u.ProjectRepository).Returns(_projectRepoMock.Object);
        }

        private ProjectService CreateService() =>
            new ProjectService(_unitOfWorkMock.Object, _mapperMock.Object, _rabbitMqMock.Object, _minioMock.Object, _httpContextMock.Object);

        // --- CreateAsync ---
        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenNoUser()
        {
            _httpContextMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

            var service = CreateService();
            await Assert.ThrowsAsync<HttpExeption>(() => service.CreateAsync(new CreateProjectDTO()));
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateProject_WhenUserValid()
        {
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123")
            }, "mock"));
            var context = new DefaultHttpContext { User = claims };
            _httpContextMock.Setup(x => x.HttpContext).Returns(context);

            var dto = new CreateProjectDTO { Name = "Test Project" };
            var project = new Project { Id = 1, CreaterId = 123, StatusId = 1 };

            _mapperMock.Setup(m => m.Map<Project>(dto)).Returns(project);
            _mapperMock.Setup(m => m.Map<ProjectResponseDto>(project))
                .Returns(new ProjectResponseDto { Id = 1, Name = "Test Project" });

            _projectRepoMock.Setup(r => r.Add(It.IsAny<Project>()));

            var service = CreateService();
            var result = await service.CreateAsync(dto);

            Assert.Equal(1, result.Id);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // --- GetByIdAsync ---
        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
        {
            var project = new Project { Id = 5, Name = "Found" };
            _projectRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map<ProjectResponseDto>(project))
                .Returns(new ProjectResponseDto { Id = 5, Name = "Found" });

            var service = CreateService();
            var result = await service.GetByIdAsync(5);

            Assert.Equal(5, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _projectRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Project)null);

            var service = CreateService();
            var result = await service.GetByIdAsync(99);

            Assert.Null(result);
        }

        // --- UpdateAsync ---
        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNotFound()
        {
            _projectRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Project)null);

            var service = CreateService();
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.UpdateAsync(10, new UpdateProjectDTO()));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdate_WhenFound()
        {
            var project = new Project { Id = 2, Name = "Old" };
            _projectRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(project);

            var service = CreateService();
            await service.UpdateAsync(2, new UpdateProjectDTO { Name = "New" });

            _projectRepoMock.Verify(r => r.Update(project), Times.Once);
        }

        // --- DeleteAsync ---
        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenFound()
        {
            var project = new Project { Id = 3 };
            _projectRepoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(project);

            var service = CreateService();
            await service.DeleteAsync(3);

            _projectRepoMock.Verify(r => r.DeleteByIdAsync(project), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDoNothing_WhenNotFound()
        {
            _projectRepoMock.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Project)null);

            var service = CreateService();
            await service.DeleteAsync(50);

            _projectRepoMock.Verify(r => r.DeleteByIdAsync(It.IsAny<Project>()), Times.Never);
        }

        // --- SubmitForReviewAsync ---
        [Fact]
        public async Task SubmitForReviewAsync_ShouldThrow_WhenNotFound()
        {
            _projectRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Project)null);

            var service = CreateService();
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.SubmitForReviewAsync(99));
        }

        [Fact]
        public async Task SubmitForReviewAsync_ShouldThrow_WhenStatusNotRegistered()
        {
            var project = new Project { Id = 10, StatusId = 3 };
            _projectRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(project);

            var service = CreateService();
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.SubmitForReviewAsync(10));
        }

        [Fact]
        public async Task SubmitForReviewAsync_ShouldPublish_WhenValid()
        {
            var project = new Project { Id = 7, StatusId = 1 };
            _projectRepoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(project);

            var service = CreateService();
            await service.SubmitForReviewAsync(7);

            Assert.Equal(2, project.StatusId);
            _rabbitMqMock.Verify(r => r.Publish(It.IsAny<object>(), "project-review-queue"), Times.Once);
        }

        // --- GetProjectTypesAsync ---
        [Fact]
        public async Task GetProjectTypesAsync_ShouldReturnList()
        {
            var types = new List<ProjectType> { new ProjectType { Id = 1, Name = "TypeA" } };
            _projectRepoMock.Setup(r => r.GetProjectTypeAsync()).ReturnsAsync(types);

            var service = CreateService();
            var result = await service.GetProjectTypesAsync();

            Assert.Single(result);
        }
    }
}
