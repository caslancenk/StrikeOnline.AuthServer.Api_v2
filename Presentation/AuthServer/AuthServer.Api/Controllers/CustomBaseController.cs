using AuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Api.Controllers
{
    
    public class CustomBaseController : ControllerBase
    {
        public IActionResult ActionResultInstance<T>(Response<T> response) where T : class
        {
            if (response == null)
            {
                return NotFound();
            }

            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
