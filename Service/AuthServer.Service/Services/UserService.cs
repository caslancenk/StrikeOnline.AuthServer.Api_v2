using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using AuthServer.Service.Mapper;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuthServer.Service.Services
{
    public class UserService : IUserServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Response<NoContent>> ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Response<NoContent>.Fail("Kullanıcı bulunamadı", 404, true);
            }

            // Kullanıcının mevcut rollerini al
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Tüm mevcut rollerden kullanıcıyı çıkar
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return Response<NoContent>.Fail("Mevcut roller silinirken hata oluştu", 400, true);
            }

            // Yeni rolün var olup olmadığını kontrol et
            var roleExist = await _roleManager.RoleExistsAsync(newRole);
            if (!roleExist)
            {
                return Response<NoContent>.Fail("Yeni rol mevcut değil", 404, true);
            }

            // Kullanıcıya yeni rol ekle
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                return Response<NoContent>.Fail("Yeni rol atanırken hata oluştu", 400, true);
            }

            return Response<NoContent>.Success(200); // Başarılı bir işlem 204 dönebilir
        }

        public async Task<Response<AppUserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new AppUser
            {
                UserName = createUserDto.Email.Substring(0, createUserDto.Email.IndexOf('@')),
                Email = createUserDto.Email
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<AppUserDto>.Fail(new ErrorDto(errors, true), 400); // client hatalı veri gönderdiği için 400 durum kodu
            }


            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(x => x.Description).ToList();
                return Response<AppUserDto>.Fail(new ErrorDto(errors, true), 400); // Rol atanmazsa hata döndürüyoruz
            }

            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(user), 200);

        }

        public async Task<Response<List<AppUserDto>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDto = new List<AppUserDto>();

            foreach (var user in users)
            {
                // Kullanıcının rollerini al
                var userRoles = await _userManager.GetRolesAsync(user); // Bu metod doğru kullanımdır

                userDto.Add(new AppUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = userRoles.ToList() // Kullanıcının rollerini listele
                });
            }

            return Response<List<AppUserDto>>.Success(userDto, 200);
        }

        public async Task<Response<AppUserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Response<AppUserDto>.Fail("Email not found", 404, true);
            }

            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(user), 200);
        }

        public async Task<Response<List<AppUserDto>>> GetUsersByRoleAsync(string role)
        {
            
            // Önce rolün var olup olmadığını kontrol et
            try
            { 
                // rol kontrolu yap

                // Rol bazlı kullanıcıları getir
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);

                // rolde kullanıcı varmı kontrol et

                // Kullanıcıları DTO'ya dönüştür
                var userDtos = usersInRole.Select(user => new AppUserDto
                {
                    Id = user.Id,
                    Email = user.Email,   
                    
                   
                }).ToList();

                // Başarılı sonuç döndür
                return Response<List<AppUserDto>>.Success(userDtos, 200);
            }
            catch (Exception ex)
            {
                // Hata durumunda, hata mesajını içeren bir sonuç döndür
                return Response<List<AppUserDto>>.Fail("Users not found",404,true);
            }

           

        }
    }
}





