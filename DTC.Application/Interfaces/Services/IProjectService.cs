using DTC.Application.DTO.Project;

namespace DTC.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateAsync(CreateProjectDTO createDto);
        Task<ProjectResponseDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, UpdateProjectDTO updateDto);
        Task SubmitForReviewAsync(int id);
    }
}
