using AutoMapper;
using DTC.Application.DTO;
using DTC.Application.DTO.Project;
using DTC.Domain.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTC.Application.AutoMapper.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateProjectDTO -> Project
            CreateMap<CreateProjectDTO, Project>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.VersionDate, opt => opt.MapFrom(src => src.VersionDate == default ? DateTime.UtcNow : src.VersionDate))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl ?? string.Empty))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId ?? 1)) 
                .ForMember(dest => dest.Creater, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Group, opt => opt.Ignore());

            // UpdateProjectDTO -> Project
            CreateMap<UpdateProjectDTO, Project>()
                .ForMember(dest => dest.Creater, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !(srcMember is string str)));

            // Project -> ProjectResponseDto
            CreateMap<Project, ProjectResponseDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : GetStatusName(src.StatusId)))
                .ForMember(dest => dest.CreaterName, opt => opt.MapFrom(src => src.Creater != null ? src.Creater.UserName : string.Empty))
                .ForMember(dest => dest.ProjectTypeName, opt => opt.MapFrom(src => src.Type != null ? src.Type.Name : string.Empty))
                .ForMember(dest => dest.AuthorGroupName, opt => opt.MapFrom(src => src.Group != null ? src.Group.Name : string.Empty));


            CreateMap<ProjectType, ProjectTypeDTO>();
            CreateMap<ProjectTypeDTO, ProjectType>();
            // AuthorGroup mappings
            CreateMap<CreateAuthorGroupDto, AuthorGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RegDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo ?? "default_photo.jpg"));

            CreateMap<AuthorGroup, AuthorGroupResponseDto>();
        }

        private string GetStatusName(int statusId)
        {
            return statusId switch
            {
                1 => "Registered",
                2 => "In Review",
                3 => "Approved",
                4 => "Rejected",
                5 => "Archived",
                _ => "Unknown"
            };
        }
    }
}