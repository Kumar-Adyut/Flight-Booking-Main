using CheckInService.Process;
using CheckInService.Repository;
using CommonUse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheckInService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        CheckInProcess process;
        public CheckInController(CheckInProcess process)
        {
            this.process = process;
        }

        // CHECK-IN A PASSENGER
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckInPassenger([FromBody] CheckInRequest request)
        {
            try
            {
                var response = await process.CheckInAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // GET CHECK-IN DETAILS
        [HttpGet("{checkInId}")]
        public async Task<IActionResult> GetCheckInDetails(string checkInId)
        {
            try
            {
                var response = await process.GetCheckInDetailsAsync(checkInId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
