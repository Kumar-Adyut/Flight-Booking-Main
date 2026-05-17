namespace FlightService.Repository
{
    public interface IFlight
    {
        Task<bool> AddFlight(Flight flight);
        Task<bool> RemoveFlight(int flightId);
        Task<Flight> UpdateFlight(int flightId, Flight flight);
        Task<Flight> UpdateAvailableSeat(int flightId, int availableSeat);
        Task<IEnumerable<Flight>> GetFlightByDepartureDate(string from, string to, DateOnly dateOfTravel);
        Task<Flight> GetFlightById(int flightId);
        Task<bool> ReduceAvailableSeatsAsync(int flightId, int seatsToBook);
    }
}
