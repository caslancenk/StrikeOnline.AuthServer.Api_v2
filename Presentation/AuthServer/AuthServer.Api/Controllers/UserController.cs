using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            var user = await _userServices.CreateUserAsync(createUserDto);
            return ActionResultInstance(user);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail()
        {
            return ActionResultInstance(await _userServices.GetUserByEmailAsync(HttpContext.User.FindFirst(ClaimTypes.Email)?.Value));
            //return ActionResultInstance(await _userServices.GetUserByEmailAsync(HttpContext.User.Identity.Name));
        }

        
        [HttpPost("GetUsersByRole")]
        public async Task<IActionResult> GetUsersByRole(RoleDto roleDto)
        {
            var user = await _userServices.GetUsersByRoleAsync(roleDto.Role);
            return ActionResultInstance(user);
        }


        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole(UserRoleDto userRoleDto)
        {
            var result = await _userServices.ChangeUserRoleAsync(userRoleDto.UserId, userRoleDto.NewRole);
            return ActionResultInstance(result);
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return ActionResultInstance(await _userServices.GetAllUsers());
        }
    }
}
