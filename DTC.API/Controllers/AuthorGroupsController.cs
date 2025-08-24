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
    [Authorize] // Только для авторизованных пользователей
    public class AuthorGroupsController : ControllerBase
    {
        // Здесь будет инжектироваться IAuthorGroupService, который работает с UnitOfWork
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
            // Получаем ID текущего пользователя из JWT токена.
            // Примечание: Ваш сервис использует int для ID. Убедитесь, что ClaimTypes.NameIdentifier тоже хранится в виде int.
            var creatorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(creatorIdString, out var creatorId))
            {
                return BadRequest("Invalid user ID in token.");
            }
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

                // 204 NoContent - стандартный ответ на успешное действие, не возвращающее контент.
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                // Сервис бросил исключение, что группа не найдена -> 404 Not Found
                return NotFound(ex.Message);
            }
            catch (SecurityException ex)
            {
                // Сервис бросил исключение безопасности (нет прав) -> 403 Forbidden
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Сервис бросил исключение о нарушении бизнес-логики -> 400 Bad Request
                return BadRequest(ex.Message);
            }
        }
    }
}
