
using CommonUse;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BookingService.Repository
{
    public class BookingRepository :IBooking
    {
        private readonly BookingDbContext context;
        public BookingRepository(BookingDbContext context)
        {
            this.context = context;
        }
        public async Task<string> BookFlightAsync(BookingRequest request, decimal totalFare)
        {
            try
            {
                string referenceNumber = "AB2025" + new Random().Next(1000, 9999);

                var booking = new Booking
                {
                    ReferenceNumber = referenceNumber,
                    
                    FlightId = request.FlightId,
                    TotalFare = totalFare,
                    Status = "Confirmed",
                    IsActive = true,
                    Passengers = request.Passengers.Select(p => new Passenger
                    {
                        ReferenceNumber=referenceNumber,
                        FullName = p.Name,
                        Email = p.Email,
                        Gender= p.Gender,
                        IsActive= true,
                    }).ToList()
                };

                context.Bookings.Add(booking);
                await context.SaveChangesAsync();

                return referenceNumber;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving booking: " + ex.Message);
            }
        }

        public async Task<BookingDetail> GetBookingDetailsAsync(string referenceNumber)
        {
            var booking = await context.Bookings
                .Include(b => b.Passengers)
                .FirstOrDefaultAsync(b => b.ReferenceNumber == referenceNumber);

            if (booking == null)
                throw new Exception("Booking not found.");

            return new BookingDetail
            {
                ReferenceNumber = booking.ReferenceNumber,
                TotalFare = booking.TotalFare,
                Passengers = booking.Passengers.Select(p => new PassengerDTO
                {
                    Name = p.FullName,
                    Email = p.Email,
                    Gender = p.Gender
                }).ToList()
            };
        }
        public async Task<bool> UpdateCheckInAsync(UpdateBookingSeats request)
        {
            try
            {
                // Fetch the booking by reference number
                var booking = await context.Bookings
                    .FirstOrDefaultAsync(b => b.ReferenceNumber == request.ReferenceNumber);

                // If no booking is found, return false
                if (booking == null)
                {
                    return false;
                }

                // Loop through each seat assignment in the request and update the passenger table
                // Find all passengers with the given ReferenceNumber where CheckInId is null
                var passengers = await context.Passengers
                    .Where(p => p.ReferenceNumber == request.ReferenceNumber && p.CheckInId == null)
                    .ToListAsync();

                // Ensure that the SeatAssignments list matches the number of passengers
                //if (passengers.Count != request.SeatAssignments.Count)
                //{
                //    throw new Exception("The number of passengers and seat assignments do not match.");
                //}

                foreach (var seatAssignment in request.SeatAssignments)
                {
                    // Find the corresponding passenger for each seat assignment (assumed order matches)
                    var passenger = passengers.FirstOrDefault(p => p.CheckInId == null);

                    // If the passenger is found, update their seat and CheckInId
                    if (passenger != null)
                    {
                        
                        passenger.SeatNumber = seatAssignment.SeatNumber;  
                        passenger.CheckInId = seatAssignment.CheckInId;    
                                                                        
                    }
                }

                // Save the changes to the database
                await context.SaveChangesAsync();


                // Optionally, update the booking's CheckInId and SeatNumber if needed
                var firstAssignedPassenger = request.SeatAssignments.FirstOrDefault();
                if (firstAssignedPassenger != null)
                {
                    booking.CheckInId = firstAssignedPassenger.CheckInId;  // Set the CheckInId from the first seat assignment
                    booking.SeatNumber = firstAssignedPassenger.SeatNumber;  // Set the SeatNumber from the first seat assignment
                }

                // Update the booking's checked-in status and mark it as "Checked-In"
                booking.CheckedIn = true;
                booking.Status = "Checked-In";

                // Update the booking record in the context
                context.Bookings.Update(booking);

                // Save all changes (both booking and passenger updates)
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred while updating the booking: {ex.Message}");
                return false;
            }
        }

    }
}
