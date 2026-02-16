using AutoMapper;

using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;
using Server.Domain.Entities.Core;
using Server.Domain.Interfaces.Repositories.Core;

namespace Server.Application.UseCases.Core;

public class PermissionUseCase(IPermissionRepository permissionRepository, IMapper mapper) : IPermissionUseCase
{
    public async Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync()
    {
        IEnumerable<Permission> permissions = await permissionRepository.GetAllAsync();
        return mapper.Map<IEnumerable<PermissionResponse>>(permissions.Where(p => p.IsActive));
    }

    public async Task<PermissionResponse?> GetPermissionByIdAsync(Guid id)
    {
        Permission? permission = await permissionRepository.GetByIdAsync(id);
        return permission is { IsActive: true }
            ? mapper.Map<PermissionResponse>(permission)
            : null;
    }
}
