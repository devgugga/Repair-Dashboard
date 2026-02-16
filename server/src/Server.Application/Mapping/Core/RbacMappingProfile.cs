using AutoMapper;

using Server.Application.DTOs.Response.Core;
using Server.Domain.Entities.Core;

namespace Server.Application.Mapping.Core;

public class RbacMappingProfile : Profile
{
    public RbacMappingProfile()
    {
        CreateMap<Role, RoleResponse>()
            .ForMember(dest => dest.Permissions,
                opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)));

        CreateMap<Permission, PermissionResponse>();

        CreateMap<UserRole, RoleResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Role.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Role.Description))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Role.IsActive))
            .ForMember(dest => dest.IsSystemRole, opt => opt.MapFrom(src => src.Role.IsSystemRole))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Role.CreatedAt))
            .ForMember(dest => dest.Permissions,
                opt => opt.MapFrom(src => src.Role.RolePermissions.Select(rp => rp.Permission)));
    }
}
