namespace Server.Application.DTOs.Response.Core;

public class PermissionResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Resource { get; set; }
    public required string Action { get; set; }
    public required string Description { get; set; }
    public string FullPermission { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
