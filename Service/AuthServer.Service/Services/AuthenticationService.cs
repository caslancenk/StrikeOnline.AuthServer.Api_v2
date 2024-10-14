using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationServices
    {
        private readonly List<Client> _clients;
        private readonly ITokenServices _tokenServices;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> optionsClients, ITokenServices tokenServices, UserManager<AppUser> userManager,
                                                            IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClients.Value;
            _tokenServices = tokenServices;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            //if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            //var user = await _userManager.FindByEmailAsync(loginDto.Email);
            //if (user == null) 
            //{ 
            //    return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            //}

            //if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) 
            //{
            //    return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            //}

            //var token = _tokenServices.CreateToken(user);

            //// veri tabanı kontrol ediyor daha önce refresh token üretildimi diye
            //var userRefreshToken = await _userRefreshTokenService.Where(x=>x.UserId == user.Id).SingleOrDefaultAsync();

            //if (userRefreshToken == null)
            //{
            //    // refresh token database ekleniyor
            //    await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiraton = token.RefreshTokenExpiration });
            //}
            //else
            //{// database te refresh token varsa güncelleme işlemi 
            //    userRefreshToken.Code = token.RefreshToken;
            //    userRefreshToken.Expiraton = token.RefreshTokenExpiration;
            //}

            //await _unitOfWork.CommitAsync();

            //return Response<TokenDto>.Success(token, 200);

            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            }

            var token = await _tokenServices.CreateTokenAsync(user); // Asenkron çağrı

            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiraton = token.RefreshTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiraton = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(token, 200);

        }

        public  Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {

            var client = _clients.SingleOrDefault(x => x.ClientId == clientLoginDto.ClientId && x.ClientSecret == clientLoginDto.ClientSecret);

            if (client == null)
            {
                return Response<ClientTokenDto>.Fail("Client or ClientSecret not found", 404, true);
            }

            var token = _tokenServices.CreateTokenByClient(client);

            return Response<ClientTokenDto>.Success(token, 200);

        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x=>x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null) 
            {
                return Response<TokenDto>.Fail("Refresh Token not Found",404,true);
            }

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user == null) 
            {
                return Response<TokenDto>.Fail("User not Found", 404, true);
            }

            var tokenDto = await _tokenServices.CreateTokenAsync(user);

            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiraton = tokenDto.RefreshTokenExpiration;
                        
            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken == null) 
            {
                return Response<NoDataDto>.Fail("Refresh Token not Found", 404, true);
            }

            _userRefreshTokenService.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }
}
