using DTC.Application.DTO.Project;
using DTC.Domain.Entities.Main;
using System.Security.Claims;

namespace DTC.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateAsync(CreateProjectDTO createDto,ClaimsPrincipal user);
        Task<ProjectResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectType>> GetProjectTypesAsync();
        Task UpdateAsync(int id, UpdateProjectDTO updateDto);
        Task SubmitForReviewAsync(int id);
        Task DeleteAsync(int id);
    }
}
