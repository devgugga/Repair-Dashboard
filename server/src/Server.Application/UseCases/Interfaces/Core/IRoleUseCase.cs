using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;

namespace Server.Application.UseCases.Interfaces.Core;

public interface IRoleUseCase
{
    Task<IEnumerable<RoleResponse>> GetAllRolesAsync();
    Task<RoleResponse?> GetRoleByIdAsync(Guid id);
    Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, Guid currentUserId);
    Task<RoleResponse> UpdateRoleAsync(Guid id, UpdateRoleRequest request, Guid currentUserId);
    Task DeleteRoleAsync(Guid id);
}
