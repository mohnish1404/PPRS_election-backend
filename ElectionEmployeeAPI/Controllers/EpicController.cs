using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ElectionEmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpicController : ControllerBase
    {
        [HttpGet("verify/{epicNo}")]
        public IActionResult VerifyEpic(string epicNo)
        {
            var pattern = @"^[A-Z]{3}[0-9]{7}$";

            if (Regex.IsMatch(epicNo, pattern)) // 👈 yaha fix
            {
                return Ok(new { valid = true });
            }
            else
            {
                return Ok(new { valid = false });
            }
        }
    }
}