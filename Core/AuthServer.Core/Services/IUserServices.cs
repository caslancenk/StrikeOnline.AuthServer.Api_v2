using AuthServer.Core.Dtos;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IUserServices
    {
        Task<Response<AppUserDto>> CreateUserAsync(CreateUserDto createUserDto);

        Task<Response<AppUserDto>> GetUserByEmailAsync(string email);

        Task<Response<List<AppUserDto>>> GetUsersByRoleAsync(string role);
        Task<Response<List<AppUserDto>>> GetAllUsers();

        Task<Response<NoContent>> ChangeUserRoleAsync(string userId, string newRole);
    }
}
