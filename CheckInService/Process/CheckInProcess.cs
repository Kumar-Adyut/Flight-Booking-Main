
using CheckInService.Repository;
using CommonUse;
using Models;
using System.Text.Json;

namespace CheckInService.Process
{
    public class CheckInProcess
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ICheckIn _checkInRepository;

        public CheckInProcess(HttpClient httpClient, IConfiguration configuration, ICheckIn checkInRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _checkInRepository = checkInRepository;
        }

        public async Task<List<CheckInResponse>> CheckInAsync(CheckInRequest request)
        {
            string bookingServiceUrl = _configuration["ServiceUrls:BookingService"];

            // Verify booking reference from Booking Service
            var bookingResponse = await _httpClient.GetAsync($"{bookingServiceUrl}/api/booking/{request.ReferenceNumber}");
            if (!bookingResponse.IsSuccessStatusCode)
            {
                throw new Exception("Invalid booking reference.");
            }
            //var response= await _checkInRepository.CheckInAsync(request);
            //// Update booking status in Booking Service
            //var updateInBooking = await _httpClient.PutAsJsonAsync($"{bookingServiceUrl}/api/booking/update-checkin/{request.ReferenceNumber}", new UpdateCheckInRequest { CheckInId = response.CheckInId, SeatNumber = response.SeatNumber });

            //if (!updateInBooking.IsSuccessStatusCode)
            //    throw new Exception("Failed to update booking status in Booking Service.");
            //return response;


            var bookingDetails = await bookingResponse.Content.ReadFromJsonAsync<BookingDetail>();
            if (bookingDetails == null || bookingDetails.Passengers == null || bookingDetails.Passengers.Count == 0)
            {
                throw new Exception("No passengers found for this booking.");
            }

            // Map the list of passengers from the BookingDetail to the CheckInRequest
            // Assuming we want to pass the passenger names, but you could map to other identifiers (like passenger IDs) if needed
            var checkInRequest = new CheckInRequest
            {
                ReferenceNumber = bookingDetails.ReferenceNumber,
                Passengers = bookingDetails.Passengers.Select(p => p.Name).ToList() // Assuming you want passenger names
            };

            // Step 2: Check-in all passengers
            var checkInResponses = await _checkInRepository.CheckInAsync(checkInRequest);

            // Step 3: Update Booking Service with seat numbers
            var updateRequest = new UpdateBookingSeats
            {
                
                ReferenceNumber = request.ReferenceNumber,
                SeatAssignments = checkInResponses.Select(c => new PassengerSeat
                {
                    CheckInId=c.CheckInId,
                    PassengerName = c.PassengerName,
                    SeatNumber = c.SeatNumber
                }).ToList()
            };

            var updateResponse = await _httpClient.PutAsJsonAsync($"{bookingServiceUrl}/api/booking/update-seats", updateRequest);
            if (!updateResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update booking service with seat assignments.");
            }

            return checkInResponses;
        }
        public async Task<CheckInResponse> GetCheckInDetailsAsync(string checkInId)
        {
            return await _checkInRepository.GetCheckInDetailsAsync(checkInId);
        }
    }
}
