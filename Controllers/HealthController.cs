using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StudentTeacherManagment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {


        // stam , ignore this guys its just for me so i can keep deployment warm and prevent cold starts
        [HttpGet]
        public IActionResult Get() => Ok("Healthy");

    }
}
