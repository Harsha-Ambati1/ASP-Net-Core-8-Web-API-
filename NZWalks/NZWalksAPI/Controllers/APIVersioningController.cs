using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion}/[controller]")] //https://localhost:7079/api/v1/apiversioning  //https://localhost:7079/api/v2/apiversioning
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class APIVersioningController : ControllerBase
    {
        //https://localhost:7079/api/apiversioning?app-version=1.0

        [HttpGet]
        [MapToApiVersion("v1")]
        public IActionResult GetV1()
        {
            return Ok("This is response from the Verson1");
        }

        //https://localhost:7079/api/apiversioning?app-version=2.0
        [HttpGet]
        [MapToApiVersion("v2")]
        public IActionResult GetV2()
        {
            return Ok("This is response from the Verson2");
        }
    }
}
