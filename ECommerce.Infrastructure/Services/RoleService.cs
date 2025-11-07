using ECommerce.Core.DTOs.Auth;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleService(RoleManager<IdentityRole> _roleManager)
        {
            roleManager = _roleManager;
        }

        public async Task<Result> AddRole(RoleDto roleDto)
        {
            IdentityRole role = new IdentityRole();
            role.Name = roleDto.RoleName;
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                string Error = "Role";
                foreach (var item in result.Errors)
                {
                   Error +=item.Description;
                }
                return Result.Failure(Error);
            }
            return Result.Success();
        }
    }
}
