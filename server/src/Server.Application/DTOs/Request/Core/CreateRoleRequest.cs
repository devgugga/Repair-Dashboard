namespace Server.Application.DTOs.Request.Core;

public class CreateRoleRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = [];
}
