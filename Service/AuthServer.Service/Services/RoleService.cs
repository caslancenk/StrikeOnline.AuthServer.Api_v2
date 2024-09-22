using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using AuthServer.Service.Mapper;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class RoleService : IRoleServices
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Response<AppRoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            var role = new AppRole 
            {
                Name = createRoleDto.Name 
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x=>x.Description).ToList();
                return Response<AppRoleDto>.Fail(new ErrorDto(errors,true),400);
            }

            return Response<AppRoleDto>.Success(ObjectMapper.Mapper.Map<AppRoleDto>(role),200);
        }

        public async Task<Response<List<AppRoleDto>>> GetAllAsync()
        {
            var roleList = await _roleManager.Roles.ToListAsync();

            var roleDto = roleList.Select(role => new AppRoleDto
            {
                Name = role.Name
            }).ToList();

            return Response<List<AppRoleDto>>.Success(roleDto,200);

        }
    }
}

