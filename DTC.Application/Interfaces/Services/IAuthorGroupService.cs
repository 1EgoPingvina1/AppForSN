using DTC.Application.DTO;

namespace DTC.Application.Interfaces.Services
{
    public interface IAuthorGroupService
    {
        Task<AuthorGroupResponseDto> CreateGroupAsync(CreateAuthorGroupDto createDto, int creatorUserId);
        Task AddMemberAsync(int groupId, int userIdToAdd, int currentUserId);
        Task<AuthorGroupResponseDto?> GetGroupByIdAsync(int id);
    }
}
