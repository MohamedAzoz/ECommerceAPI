using ECommerce.Core.DTOs.Auth;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService _roleService)
        {
            roleService = _roleService;
        }
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await roleService.AddRole(roleDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            return Created();
        }
    }
}
