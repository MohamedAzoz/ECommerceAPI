using ECommerce.Core.DTOs.Auth;
using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface IRoleService
    {
        public Task<Result> AddRole(RoleDto roleDto);
    }
}
