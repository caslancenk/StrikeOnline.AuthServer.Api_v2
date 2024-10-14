using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using AuthServer.SharedLibrary.Configuration;
using AuthServer.SharedLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace AuthServer.Service.Services
{
    internal class TokenService : ITokenServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly CustomTokenOptions _tokenOptions;

        public TokenService(UserManager<AppUser> userManager, IOptions<CustomTokenOptions> options)
        {
            _userManager = userManager;
            _tokenOptions = options.Value;
        }

        // refresh token üretme fonksyonu
        private string CreateRefreshToken()
        {
            var numberByte = new byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }


        // cliamler --> tokenın payload kısmıda ki değerler --> üyelik sistemi gerektiren token
        private async Task<IEnumerable<Claim>> GetClaims(AppUser appUser,List<string> audiences) 
        {

            //Console.WriteLine($"User Email: {appUser.Email}");
            var userRoles = await _userManager.GetRolesAsync(appUser);


            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(ClaimTypes.Email, appUser.Email),
                //new Claim(JwtRegisteredClaimNames.Email,appUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()) // her token için token id olusturuyorum zorunlu değil
            };

            userList.AddRange(audiences.Select(x=>new Claim(JwtRegisteredClaimNames.Aud,x)));
            userList.AddRange(userRoles.Select(x=>new Claim(ClaimTypes.Role,x)));


            return userList;           
        }

        // üyelik sistemi gerektirmeyen token payload kısmı
        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString());

            return claims;
        }

        public async Task<TokenDto> CreateTokenAsync(AppUser appUser)
        {
            // Kullanıcı bilgilerini kontrol etmek için
            //Console.WriteLine($"Creating token for User: {appUser.Id}, Email: {appUser.Email}");

            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.RefreshTokenExpiration);
            //var securityKey = SignService.GetSymmetricSecurityKey(_tokenOptions.SecurtyKey);
            //SigningCredentials signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature);
            SigningCredentials signingCredentials = new SigningCredentials(SignServices.GetSymmetricSecurityKey(_tokenOptions.SecurityKey),SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                signingCredentials: signingCredentials,
                claims: await GetClaims(appUser,_tokenOptions.Audience)
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshToken = CreateRefreshToken(),
                RefreshTokenExpiration = refreshTokenExpiration
            };
            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            SigningCredentials signingCredentials = new SigningCredentials(SignServices.GetSymmetricSecurityKey(_tokenOptions.SecurityKey), SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                signingCredentials: signingCredentials,
                claims: GetClaimsByClient(client)
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var clientTokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,               
            };

            return clientTokenDto;
        }

  
    }
}
