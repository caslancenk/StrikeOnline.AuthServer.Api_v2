using AuthServer.Core.Dtos;
using AuthServer.SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IRoleServices
    {
        Task<Response<AppRoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);

        Task<Response<List<AppRoleDto>>> GetAllAsync();
    }
}
