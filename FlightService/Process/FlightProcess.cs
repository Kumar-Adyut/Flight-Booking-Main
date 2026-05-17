using CommonUse;
using FlightService.Repository;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Process
{
    public class FlightProcess
    {
        IFlight _repository;
        public FlightProcess(IFlight repository)
        {
            this._repository = repository;
        }
        public async Task<bool> AddFlight(Flight flight)
        {
            return await _repository.AddFlight(flight);
        }
        public async Task<IEnumerable<Flight>> GetFlights(string from, string to, DateOnly departure)
        {
            var res = await _repository.GetFlightByDepartureDate(from, to, departure);
            if (!res.Any())
            {
                throw new KeyNotExistException("Flights not exist");
            }
            return res;
        }
        public async Task<bool> RemoveFlight(int flightId)
        {
            return await _repository.RemoveFlight(flightId);

        }
        public async Task<Flight> GetFlightById(int flightId)
        {
            var res= await _repository.GetFlightById(flightId);
            if (res==null)
            {
                throw new Exception("Flight not exist");
            }
            return res;

        }
        public async Task<Flight> UpdateAvailableSeat(int flightId, int availableSeat)
        {
            return await _repository.UpdateAvailableSeat(flightId, availableSeat);
        }
        public async Task<Flight> UpdateFlight(int flightId, Flight flight)
        {
            return await _repository.UpdateFlight(flightId, flight);
        }
        public async Task<bool> ReduceSeats(int  flightId, int seatsToBook)
        {
            return await _repository.ReduceAvailableSeatsAsync(flightId, seatsToBook);
        }
    }
}
