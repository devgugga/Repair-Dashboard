namespace Server.Application.DTOs.Response.Core;

public class UserRoleResponse
{
    public Guid UserId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public List<RoleResponse> Roles { get; set; } = [];
    public List<PermissionResponse> Permissions { get; set; } = [];
}
