
using BookingService.Process;
using BookingService.Repository;
using CommonUse;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    //[CustomAuthentication(Roles ="User")]
    public class BookingController : ControllerBase
    {
        private readonly BookingProcess process;
        public BookingController(BookingProcess process)
        {
            this.process = process;
        }
        [HttpPost("book")]
        public async Task<IActionResult> BookFlight([FromBody] BookingRequest request)
        {
            try
            {
                //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                string referenceNumber = await process.BookFlightAsync(request);
                return Ok(new BookingResponse{ Message = "Booking successful", ReferenceNumber = referenceNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{referenceNumber}")]
        public async Task<IActionResult> GetBookingDetails(string referenceNumber)
        {
            try
            {
                var bookingDetails = await process.GetBookingDetailsAsync(referenceNumber);
                return Ok(bookingDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Update booking
        [HttpPut("update-seats")]
        public async Task<IActionResult> UpdateCheckIn([FromBody] UpdateBookingSeats request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                bool updateSuccess = await process.UpdateCheckIn(request);

                if (!updateSuccess)
                {
                    return StatusCode(500, "Failed to update booking status.");
                }

                return Ok("Booking status updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred while updating the booking status: {ex.Message}");
                return StatusCode(500, "An internal error occurred.");
            }
        }
    }
}
