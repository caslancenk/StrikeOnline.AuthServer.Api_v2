using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : CustomBaseController
    {
        private readonly IRoleServices _roleService;

        public RoleController(IRoleServices roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleDto createRoleDto)
        {
            var createRole = await _roleService.CreateRoleAsync(createRoleDto);
            return ActionResultInstance(createRole);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            return ActionResultInstance(await _roleService.GetAllAsync());
        }

    }
}
