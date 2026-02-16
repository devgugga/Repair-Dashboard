using System.ComponentModel.DataAnnotations;

using AutoMapper;

using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;
using Server.Domain.Entities.Core;
using Server.Domain.Exceptions.NotFound;
using Server.Domain.Interfaces.Repositories.Core;

namespace Server.Application.UseCases.Core;

public class RoleUseCase(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IMapper mapper)
    : IRoleUseCase
{
    public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync()
    {
        IEnumerable<Role> roles = await roleRepository.GetWithPermissionsAsync();
        return mapper.Map<IEnumerable<RoleResponse>>(roles);
    }

    public async Task<RoleResponse?> GetRoleByIdAsync(Guid id)
    {
        Role? role = await roleRepository.GetWithPermissionsAsync(id);
        return role != null ? mapper.Map<RoleResponse>(role) : null;
    }

    public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, Guid currentUserId)
    {
        // Validate role name doesn't exist
        if (await roleRepository.ExistsAsync(request.Name))
            throw new ValidationException("Role name already exists");

        // Validate permissions exist
        if (request.PermissionIds.Count != 0)
        {
            IEnumerable<Permission> permissions = await permissionRepository.GetByIdsAsync(request.PermissionIds);
            if (permissions.Count() != request.PermissionIds.Count)
                throw new ValidationException("One or more permissions do not exist");
        }

        // Create role
        var role = new Role { Name = request.Name, Description = request.Description };

        await roleRepository.AddAsync(role);

        // Add permissions
        if (request.PermissionIds.Count != 0)
            foreach (Guid permissionId in request.PermissionIds)
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id, PermissionId = permissionId, AssignedBy = currentUserId
                });

        await roleRepository.SaveChangesAsync();

        // Return created role with permissions
        Role? createdRole = await roleRepository.GetWithPermissionsAsync(role.Id);
        return mapper.Map<RoleResponse>(createdRole);
    }

    public async Task<RoleResponse> UpdateRoleAsync(Guid id, UpdateRoleRequest request, Guid currentUserId)
    {
        Role? role = await roleRepository.GetWithPermissionsAsync(id);
        if (role == null)
            throw new NotFoundException("Role not found");

        if (role.IsSystemRole)
            throw new ValidationException("Cannot modify system roles");

        // Validate name doesn't conflict
        if (await roleRepository.ExistsAsync(request.Name, id))
            throw new ValidationException("Role name already exists");

        // Validate permissions exist
        if (request.PermissionIds.Count != 0)
        {
            IEnumerable<Permission> permissions = await permissionRepository.GetByIdsAsync(request.PermissionIds);
            if (permissions.Count() != request.PermissionIds.Count)
                throw new ValidationException("One or more permissions do not exist");
        }

        // Update role
        role.Name = request.Name;
        role.Description = request.Description;
        role.UpdatedAt = DateTime.UtcNow;

        // Clear existing permissions
        role.RolePermissions.Clear();

        // Add new permissions
        foreach (Guid permissionId in request.PermissionIds)
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id, PermissionId = permissionId, AssignedBy = currentUserId
            });

        await roleRepository.UpdateAsync(role);
        await roleRepository.SaveChangesAsync();

        // Return updated role
        Role? updatedRole = await roleRepository.GetWithPermissionsAsync(role.Id);
        return mapper.Map<RoleResponse>(updatedRole);
    }

    public async Task DeleteRoleAsync(Guid id)
    {
        Role? role = await roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new NotFoundException("Role not found");

        if (role.IsSystemRole)
            throw new ValidationException("Cannot delete system roles");

        role.SoftDelete();
        await roleRepository.UpdateAsync(role);
        await roleRepository.SaveChangesAsync();
    }
}
