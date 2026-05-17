using System.Diagnostics;
using CommonUse;
using FlightService.Process;
using FlightService.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FlightService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class FlightController : ControllerBase
    {
        private readonly FlightProcess _process;
        public FlightController(FlightProcess process)
        {
            _process=process;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddFlight([FromBody] Flight flight)
        {
            try
            {
                var res = await _process.AddFlight(flight);
                return Ok("Flight Added Successfully");
            }
            catch (KeyAlreadyExistsException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getFlight")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlightByDepartureDate([FromQuery] string from, [FromQuery] string to, [FromQuery] DateOnly date)
        {
            try
            {
                var res = await _process.GetFlights(from, to, date);
                return Ok(res);
            }
            catch (KeyNotExistException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("del/{flightId}")]
        public async Task<IActionResult> RemoveFlight(int flightId)
        {
            try
            {
                var res = await _process.RemoveFlight(flightId);
                return Ok("Deleted successfully");
            }
            catch (KeyNotExistException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update/{flightId}")]
        public async Task<ActionResult<Flight>> UpdateAvailableSeat(int flightId,[FromQuery] int AvailableSeat)
        {
            try
            {
                var res = await _process.UpdateAvailableSeat(flightId, AvailableSeat);
                return Ok(res);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("updateFlightDetail/{flightId}")]
        public async Task<ActionResult<Flight>> UpdateFlightDetails(int flightId, [FromBody] Flight flight)
        {
            try
            {
                var res = await _process.UpdateFlight(flightId, flight);
                return Ok(res);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{flightId}")]
        public async Task<ActionResult<Flight>> GetFlightById(int flightId)
        {
            try
            {
                var res = await _process.GetFlightById(flightId);
                return Ok(res);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // Update available seats
        [HttpPut("update-seats")]
        public async Task<IActionResult> UpdateSeats([FromBody] UpdateSeatRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }
            bool updateSuccess = await _process.ReduceSeats(request.FlightId, request.SeatsToBook);

            if (!updateSuccess)
            {
                return StatusCode(500, "Failed to update available seats.");
            }

            return Ok("Seats updated successfully.");
        }
    }
}
