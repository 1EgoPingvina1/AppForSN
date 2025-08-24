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
            // Маппинг для создания проекта
            CreateMap<CreateProjectDTO, Project>();

            // Маппинг для обновления проекта
            CreateMap<UpdateProjectDTO, Project>();

            // Маппинг из сущности в DTO для ответа клиенту
            CreateMap<Project, ProjectResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetStatusName(src.StatusId)));

            CreateMap<CreateAuthorGroupDto, AuthorGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RegDate, opt => opt.Ignore())
                .ForMember(dest => dest.Members,opt => opt.Ignore())
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
                _ => "Unknown"
            };
        }
    }
    }
