using AutoMapper;
using DTC.Application.DTO;
using DTC.Application.Interfaces;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Main;
using DTC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace DTC.Infrastructure.Services
{
    public class AuthorGroupService : IAuthorGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDataBaseContext _dataBaseContext;
        private readonly IMinioFileService _minioStorage;


        public AuthorGroupService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDataBaseContext dataBaseContext = null, IMinioFileService minioStorage = null)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dataBaseContext = dataBaseContext;
            _minioStorage = minioStorage;
        }

        public async Task<AuthorGroupResponseDto> CreateGroupAsync(CreateAuthorGroupDto createDto, int creatorUserId)
        {
            var user = await _dataBaseContext.Users.FirstOrDefaultAsync(u => u.Id == creatorUserId);
            var author = await _unitOfWork.AuthorsRepository.GetByUserIdAsync(creatorUserId);
            if (author == null)
            {
                author = new Author
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserId = creatorUserId,
                    RegDate = DateTime.UtcNow
                };
                _unitOfWork.AuthorsRepository.Add(author);
            }

            await _unitOfWork.SaveChangesAsync();


            var group = _mapper.Map<AuthorGroup>(createDto);
            group.RegUserId = creatorUserId;
            group.RegDate = DateTime.UtcNow;

            if (createDto.Photo != null)
            {
                using Stream stream = createDto.Photo.OpenReadStream();

                string objectName = $"{Guid.NewGuid()}_{createDto.Photo.FileName}";

                string url = await _minioStorage.UploadFileAsync(stream, objectName, createDto.Photo.ContentType, "groups-photos");

                group.Photo = url;
            }
            else
            {
                group.Photo = "default_group_photo.jpg";
            }

            _unitOfWork.AuthorGroupsRepository.Add(group);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AuthorGroupResponseDto>(group);
        }

        public async Task AddMemberAsync(int groupId, int userIdToAdd, int currentUserId)
        {
            var group = await _unitOfWork.AuthorGroupsRepository.GetByIdWithMembersAsync(groupId);
            if (group == null)
            {
                throw new KeyNotFoundException($"Group with ID {groupId} not found.");
            }

            bool isCurrentUserMember = group.Members.Any(m => m.Author.UserId == currentUserId);
            if (!isCurrentUserMember)
            {
                throw new SecurityException("Only group members can add new members.");
            }

            // Проверяем, не является ли добавляемый пользователь уже участником
            bool isUserAlreadyMember = group.Members.Any(m => m.Author.UserId == userIdToAdd);
            if (isUserAlreadyMember)
            {
                throw new InvalidOperationException("This user is already a member of the group.");
            }

            // Находим или создаем профиль автора для нового участника
            var authorToAdd = await _unitOfWork.AuthorsRepository.GetByUserIdAsync(userIdToAdd);
            if (authorToAdd == null)
            {
                authorToAdd = new Author { UserId = userIdToAdd, RegDate = DateTime.UtcNow };
                _unitOfWork.AuthorsRepository.Add(authorToAdd);
            }

            // Добавляем нового участника
            group.Members.Add(new AuthorGroupMember
            {
                Author_ID = authorToAdd.Id,
                AuthorGroup_ID = group.Id,
                JoinDate = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AuthorGroupResponseDto?> GetGroupByIdAsync(int id)
        {
            var group = await _unitOfWork.AuthorGroupsRepository.GetByIdAsync(id);
            return _mapper.Map<AuthorGroupResponseDto>(group);
        }
    }
}
