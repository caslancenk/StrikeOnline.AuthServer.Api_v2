using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuthServer.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthControlller : CustomBaseController
    {
        private readonly IAuthenticationServices _authenticationServices;

        public AuthControlller(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var token = await _authenticationServices.CreateTokenAsync(loginDto);
            return ActionResultInstance(token);
        }

        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto) 
        {
            var clientToken = _authenticationServices.CreateTokenByClient(clientLoginDto);

            return ActionResultInstance(clientToken);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var refreshToken =await _authenticationServices.RevokeRefreshTokenAsync(refreshTokenDto.Token);
            return ActionResultInstance(refreshToken);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var refreshToken = await _authenticationServices.CreateTokenByRefreshTokenAsync(refreshTokenDto.Token);
            return ActionResultInstance(refreshToken);
        }
    }
}
