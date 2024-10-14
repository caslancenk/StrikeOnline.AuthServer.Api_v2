using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;


namespace AuthServer.Core.Services
{
    public interface ITokenServices
    {
        //Task<TokenDto> CreateToken(AppUser appUser);
        Task<TokenDto> CreateTokenAsync(AppUser appUser);


        ClientTokenDto CreateTokenByClient(Client client);
    }
}
