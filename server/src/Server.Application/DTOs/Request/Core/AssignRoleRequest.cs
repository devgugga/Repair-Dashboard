namespace Server.Application.DTOs.Request.Core;

public class AssignRoleRequest
{
    public required List<Guid> RoleIds { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}
