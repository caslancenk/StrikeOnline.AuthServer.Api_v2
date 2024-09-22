using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using AuthServer.Service.Mapper;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Identity;


namespace AuthServer.Service.Services
{
    public class UserService : IUserServices
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<AppUserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new AppUser
            {
                Email = createUserDto.Email,
                UserName = createUserDto.Email.Substring(0, createUserDto.Email.IndexOf('@')),
            };

            var result = await _userManager.CreateAsync(user,createUserDto.Password);

            if (!result.Succeeded) 
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<AppUserDto>.Fail(new ErrorDto(errors,true),400); // client hatalı veri gönderdiği için 400 durum kodu
            }


            var roleResult = await _userManager.AddToRoleAsync(user,"user");

            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(x => x.Description).ToList();
                return Response<AppUserDto>.Fail(new ErrorDto(errors, true), 400); // Rol atanmazsa hata döndürüyoruz
            }

            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(user),200);

        }

        public async Task<Response<AppUserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Response<AppUserDto>.Fail("Email not found",404,true);
            }

            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(user), 200);
        }
    }
}
