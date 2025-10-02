using DTC.Application.DTO.Project;
using DTC.Domain.Entities.Main;

namespace DTC.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateAsync(CreateProjectDTO createDto);
        Task<ProjectResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectType>> GetProjectTypesAsync();
        Task UpdateAsync(int id, UpdateProjectDTO updateDto);
        Task SubmitForReviewAsync(int id);
        Task DeleteAsync(int id);
    }
}
