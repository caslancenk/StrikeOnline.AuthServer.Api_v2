using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiniApp.API.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {
            ////var userName = HttpContext.User.Identity.Name;

            ////var userEmail = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email);
            ////var userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email);
            //var userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"); 


            //var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            //// veritabanında userid ile gerekli dataları çek

            //return Ok($"Email:{userEmailClaim.Value} - UserId:{userIdClaim.Value}");


            // Claim'leri konsola yazdır
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            var userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (userEmailClaim == null)
            {
                return BadRequest("Email claim'i bulunamadı.");
            }

            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Email: {userEmailClaim.Value} - UserId: {userIdClaim.Value}");
        }
    }
}
