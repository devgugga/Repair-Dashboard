using AutoMapper;

using FluentValidation;

using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;
using Server.Domain.Entities.Core;
using Server.Domain.Exceptions.NotFound;
using Server.Domain.Interfaces.Repositories;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Domain.Interfaces.Services.Core;

namespace Server.Application.UseCases.Core;

public class UserRoleUseCase(
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository,
    IBaseRepository<User> userRepository,
    IRbacService rbacService,
    IMapper mapper)
    : IUserRoleUseCase
{
    public async Task<UserRoleResponse?> GetUserRolesAsync(Guid userId)
    {
        User? user = await userRepository.GetByIdAsync(userId);
        if (user is not { IsActive: true })
            return null;

        IEnumerable<Role> userRoles = await rbacService.GetUserRolesAsync(userId);
        IEnumerable<Permission> userPermissions = await rbacService.GetUserPermissionsAsync(userId);

        return new UserRoleResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Person.Email,
            Roles = mapper.Map<List<RoleResponse>>(userRoles),
            Permissions = mapper.Map<List<PermissionResponse>>(userPermissions)
        };
    }

    public async Task AssignRolesToUserAsync(Guid userId, AssignRoleRequest request, Guid currentUserId)
    {
        // Validate user exists
        User? user = await userRepository.GetByIdAsync(userId);
        if (user is not { IsActive: true })
            throw new NotFoundException("User not found");

        // Validate roles exist
        IEnumerable<Role> roles = await roleRepository.GetByIdsAsync(request.RoleIds);
        if (roles.Count() != request.RoleIds.Count)
            throw new ValidationException("One or more roles do not exist");

        // Get current user roles to avoid duplicates
        IEnumerable<UserRole> currentUserRoles = await userRoleRepository.GetByUserIdAsync(userId);
        IEnumerable<UserRole> userRoles = currentUserRoles as UserRole[] ?? currentUserRoles.ToArray();
        var currentRoleIds = userRoles.Select(ur => ur.RoleId).ToHashSet();

        // Assign new roles
        foreach (Guid roleId in request.RoleIds.Where(id => !currentRoleIds.Contains(id)))
            await rbacService.AssignRoleToUserAsync(userId, roleId, currentUserId);

        // Update expiration for existing roles if provided
        if (request.ExpiresAt.HasValue)
            foreach (UserRole userRole in userRoles.Where(ur => request.RoleIds.Contains(ur.RoleId)))
            {
                userRole.ExpiresAt = request.ExpiresAt;
                await userRoleRepository.UpdateAsync(userRole);
            }

        await userRoleRepository.SaveChangesAsync();
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        // Validate user exists
        User? user = await userRepository.GetByIdAsync(userId);
        if (user is not { IsActive: true })
            throw new NotFoundException("User not found");

        // Validate role exists
        Role? role = await roleRepository.GetByIdAsync(roleId);
        if (role is not { IsActive: true })
            throw new NotFoundException("Role not found");

        await rbacService.RemoveRoleFromUserAsync(userId, roleId);
    }
}
