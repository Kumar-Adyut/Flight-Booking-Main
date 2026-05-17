
using BookingService.Repository;
using CommonUse;
using FlightService;
using Microsoft.Extensions.Configuration;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BookingService.Process
{
    public class BookingProcess
    {
        private readonly IBooking _bookingRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public BookingProcess(IBooking bookingRepository, HttpClient httpClient, IConfiguration configuration)
        {
            _bookingRepository = bookingRepository;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> BookFlightAsync(BookingRequest request)
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); //calling second api
                // Fetch base fare from Fare Service
                string fareServiceUrl = _configuration["ServiceUrls:FareService"];

                var fareResponse = await _httpClient.GetFromJsonAsync<decimal>($"{fareServiceUrl}/api/Fare/get-fare-by-flight/{request.FlightId}");

                if (fareResponse == 0)
                    throw new Exception("Invalid fare response from Fare Service.");

                // Fetch airport charges from City Service
                //string cityServiceUrl = _configuration["ServiceUrls:CityService"];
                //var airportCharge = await _httpClient.GetFromJsonAsync<decimal>($"{cityServiceUrl}/api/city/airportcharge/{request.FlightId}");

                //if (airportCharge == 0)
                //    throw new Exception("Invalid airport charge from City Service.");

                // 1️⃣ Fetch Airport Charge from City Service
                //var cityResponse = await httpClient.GetFromJsonAsync<CityDTO>($"http://localhost:5001/api/cities/{request.SourceCity}");
                //decimal airportCharge = cityResponse?.AirportCharge ?? 0;

                // Calculate total fare
                //decimal totalFare = fareResponse + airportCharge;
                decimal totalFare = request.Passengers.Count * fareResponse;

                //string flightServiceUrl = _configuration["ServiceUrls:FlightService"];
                //var getFlightNumResponse = await _httpClient.GetFromJsonAsync<Flight>($"{flightServiceUrl}/api/flight/{request.FlightId}");


                // Call Repository to book flight
                string referenceNumber = await _bookingRepository.BookFlightAsync(request, totalFare);

                // Decrease available seats in Flight Service
                //string flightServiceUrl = _configuration["ServiceUrls:FlightService"];
                //var updateSeatsResponse = await _httpClient.PutAsJsonAsync($"{flightServiceUrl}/api/flights/update-seats/{request.FlightId}",
                //    new { SeatsToReduce = request.Passengers.Count });

                //if (!updateSeatsResponse.IsSuccessStatusCode)
                //    throw new Exception("Failed to update available seats in Flight Service.");

                return referenceNumber;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while processing booking: " + ex.Message);
            }
        }

        public async Task<BookingDetail> GetBookingDetailsAsync(string referenceNumber)
        {
            return await _bookingRepository.GetBookingDetailsAsync(referenceNumber);
        }
        //public async Task<bool> AddPassenger(Passenger passenger)
        //{
        //    return await repo.AddPassenger(passenger);
        //}
        public async Task<bool> UpdateCheckIn(UpdateBookingSeats request)
        {
            return await _bookingRepository.UpdateCheckInAsync(request);
        }
    }
}
