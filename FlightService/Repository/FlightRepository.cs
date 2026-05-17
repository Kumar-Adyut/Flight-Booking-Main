
using CommonUse;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Repository
{
    public class FlightRepository : IFlight
    {
        FlightDbContext _context;
        public FlightRepository(FlightDbContext cntxt)
        {
            _context = cntxt;
        }
        public async Task<bool> AddFlight(Flight flight)
        {
            try
            {
                if (flight == null) throw new ArgumentException($"{flight} Enter valid Data");
                var res = await _context.Flights.AnyAsync(c => c.FlightId == flight.FlightId && c.IsActive);
                if (res) throw new KeyAlreadyExistsException("Id Already Exist");
                _context.Flights.Add(flight);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Flight>> GetFlightByDepartureDate(string from, string to, DateOnly dateOfTravel)
        {
            try
            {
                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) throw new ArgumentException("Data can't be null");
                var res = await _context.Flights
                    .Where(c => c.FromCity == from && c.ToCity == to && c.DepartureDate == dateOfTravel && c.IsActive)
                    .ToListAsync();
                return res;
            }
            catch (KeyNotExistException) { throw; }
        }

        public async Task<Flight> GetFlightById(int flightId)
        {
            try
            {
                var res = await _context.Flights.FirstOrDefaultAsync(c => c.FlightId == flightId && c.IsActive);
                if (res is null) throw new KeyNotFoundException("Id not found");
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveFlight(int flightId)
        {
            try
            {
                if (flightId < 0) throw new ArgumentOutOfRangeException("Id can't be < 0");
                var res = await _context.Flights
                    .FirstOrDefaultAsync(c => c.FlightId == flightId && c.IsActive);
                if (res is null) throw new KeyNotExistException("Id not found");
                //dbContext.Flights.Remove(res);
                res.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }

        public async Task<Flight> UpdateAvailableSeat(int flightId, int availableSeat)
        {
            try
            {
                if (flightId < 0 || availableSeat < 0) throw new ArgumentException("Data can't be null");
                var res = await _context.Flights.FirstOrDefaultAsync(c => c.FlightId == flightId && c.IsActive);
                if (res is null) throw new KeyNotExistException("Id not found");
                res.AvailableSeats = availableSeat;
                await _context.SaveChangesAsync();
                return res;
            }
            catch (Exception) { throw; }
        }

        public async Task<Flight> UpdateFlight(int flightId, Flight flight)
        {
            try
            {

                if (flightId != flight.FlightId) throw new Exception("Id not match");
                if (flight is null) throw new ArgumentException("Data can't be null");
                var res = await _context.Flights.FirstOrDefaultAsync(c => c.FlightId == flightId && c.IsActive);
                if (res is null) throw new KeyNotFoundException("Id not found");
                
                res.FlightNo = flight.FlightNo;
                res.FromCity = flight.FromCity;
                res.ToCity = flight.ToCity;
                res.DepartureDate = flight.DepartureDate;
                res.DepartureTime = flight.DepartureTime;
                res.ArrivalTime = flight.ArrivalTime;
                res.AvailableSeats=flight.AvailableSeats;
                await _context.SaveChangesAsync();
                return res;
            }
            catch (Exception) { throw; }
        }
        

        // REDUCE AVAILABLE SEATS AFTER SUCCESSFUL BOOKING
        public async Task<bool> ReduceAvailableSeatsAsync(int flightId, int seatsToBook)
        {
            try
            {
                var flight = await _context.Flights.FindAsync(flightId);
                if (flight == null || flight.AvailableSeats < seatsToBook)
                    return false; // Not enough seats available

                flight.AvailableSeats -= seatsToBook;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error reducing available seats: " + ex.Message);
            }
        }
    }
}
