using Server.Application.DTOs.Response.Core;

namespace Server.Application.UseCases.Interfaces.Core;

public interface IPermissionUseCase
{
    Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync();
    Task<PermissionResponse?> GetPermissionByIdAsync(Guid id);
}
