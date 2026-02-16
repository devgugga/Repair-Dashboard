namespace Server.Application.DTOs.Response.Core;

public class RoleResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemRole { get; set; }
    public List<PermissionResponse> Permissions { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
