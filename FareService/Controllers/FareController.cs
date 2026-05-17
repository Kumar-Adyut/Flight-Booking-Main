using CommonUse;
using FareService.Process;
using FareService.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FareService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class FareController : ControllerBase
    {
        private readonly FareProcess _process;

        public FareController(FareProcess proc)
        {
            _process = proc;
        }

        // POST: api/Fare/AddFare
        [HttpPost("add-fare")]
        public async Task<IActionResult> AddFareForFlight([FromBody] Fare fare)
        {
            try
            {
                var result = await _process.AddFare(fare);
                if (result)
                    return Ok("Fare added successfully.");
                return BadRequest("Failed to add fare.");
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Fare/RemoveFare/{fareId}
        [HttpDelete("remove-fare/{fareId}")]
        public async Task<IActionResult> RemoveFareForFlight(int fareId)
        {
            try
            {
                var result = await _process.RemoveFare(fareId);
                if (result)
                    return Ok("Fare removed successfully.");
                return NotFound("Fare not found.");
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Fare/UpdateFareByFareId
        [HttpPut("update-fare/{fareId}")]
        public async Task<IActionResult> UpdateFareByFareId(int fareId, [FromBody] Fare fare)
        {
            try
            {
                var updatedFare = await _process.UpdateFareByFareId(fareId, fare.BasePrice, fare.ConvenienceFee);
                if (updatedFare == null)
                    return NotFound("Fare not found.");
                return Ok(updatedFare);
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Fare/UpdateFareByFlightId
        [HttpPut("update-fare-by-flight/{flightId}")]
        public async Task<IActionResult> UpdateFareByFlightId(int flightId, [FromBody] Fare fare)
        {
            try
            {
                var updatedFare = await _process.UpdateFareByFlightId(flightId, fare.BasePrice, fare.ConvenienceFee);
                if (updatedFare == null)
                    return NotFound("Fare not found.");
                return Ok(updatedFare);
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Fare/GetFareByFlightId/{flightId}
        //[CustomAuthentication(Roles ="User")]
        [HttpGet("get-fare-by-flight/{flightId}")]
        public async Task<IActionResult> GetFareByFlightId(int flightId)
        {
            try
            {
                var fare = await _process.GetFareByFlightId(flightId);
                if (fare == 0)
                    return NotFound("Fare not found.");
                return Ok(fare);
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Fare/GetFareByFareID/{fareId}
        [HttpGet("get-fare-by-id/{fareId}")]
        public async Task<IActionResult> GetFareByFareID(int fareId)
        {
            try
            {
                var fare = await _process.GetFareByFareID(fareId);
                if (fare == 0)
                    return NotFound("Fare not found.");
                return Ok(fare);
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Fare/GetTotalFare/{fareId}
        [HttpGet("get-total-fare/{fareId}")]
        public async Task<IActionResult> GetTotalFare(int fareId)
        {
            try
            {
                var totalFare = await _process.GetTotalFare(fareId);
                return Ok(totalFare);
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
