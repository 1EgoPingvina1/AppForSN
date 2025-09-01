using DTC.Application.DTO;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Security.Claims;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthorGroupsController : ControllerBase
    {
        private readonly IAuthorGroupService _authorGroupService;

        public AuthorGroupsController(IAuthorGroupService authorGroupService)
        {
            _authorGroupService = authorGroupService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AuthorGroupResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var group = await _authorGroupService.GetGroupByIdAsync(id);

            if (group == null)
            {
                return NotFound($"Group with ID {id} not found.");
            }

            return Ok(group);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthorGroupResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGroup([FromBody] CreateAuthorGroupDto createDto)
        {
            var creatorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(creatorIdString, out var creatorId))
                return BadRequest("Invalid user ID in token.");
            await _authorGroupService.CreateGroupAsync(createDto, creatorId);
            return Ok();
        }

        [HttpPost("{groupId}/members")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMember(int groupId, [FromBody] AddMemberRequestDto addDto)
        {
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out var currentUserId))
            {
                return BadRequest("Invalid user ID in token.");
            }
            try
            {
                await _authorGroupService.AddMemberAsync(groupId, addDto.UserId, currentUserId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (SecurityException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}