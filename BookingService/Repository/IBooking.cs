
using CommonUse;

namespace BookingService.Repository
{
    public interface IBooking
    {
        Task<string> BookFlightAsync(BookingRequest request, decimal totalFare);
        Task<BookingDetail> GetBookingDetailsAsync(string referenceNumber);
        Task<bool> UpdateCheckInAsync(UpdateBookingSeats request);
    }
}
