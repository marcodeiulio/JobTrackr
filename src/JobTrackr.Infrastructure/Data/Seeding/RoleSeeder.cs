using JobTrackr.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace JobTrackr.Infrastructure.Data.Seeding;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger logger)
    {
        logger.LogInformation("Seeding Identity Roles");

        await SeedRoleAsync(roleManager, Roles.Admin, logger);
        await SeedRoleAsync(roleManager, Roles.User, logger);
    }

    private static async Task SeedRoleAsync(
        RoleManager<IdentityRole<Guid>> roleManager,
        string roleName,
        ILogger logger)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            logger.LogInformation("Role {RoleName} already exists", roleName);
            return;
        }

        var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

        if (result.Succeeded)
        {
            logger.LogInformation("Created role {RoleName}", roleName);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, errors);
            throw new InvalidOperationException($"Failed to seed role {roleName}: {errors}");
        }
    }
}