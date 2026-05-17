namespace FlightService.Controllers
{
    public class UpdateSeatRequest
    {
        public int FlightId { get; set; }
        public int SeatsToBook { get; set; }
    }
}
