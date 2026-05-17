namespace CommonUse;
public class BookingRequest
{
    public int FlightId { get; set; }
    public List<PassengerDTO> Passengers { get; set; }
}
public class BookingDetail
{
    public string ReferenceNumber { get; set; }
    public decimal TotalFare { get; set; }
    public List<PassengerDTO> Passengers { get; set; }
}
public class PassengerDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
}
public class BookingResponse
{
    public string Message { get; set; }
    public string ReferenceNumber { get; set; }
}

