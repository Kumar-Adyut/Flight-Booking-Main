namespace CommonUse

{
    public class CheckInResponse
    {
        public string CheckInId { get; set; }
        public string ReferenceNumber { get; set; }
        public string PassengerName { get; set; }
        public string SeatNumber { get; set; }
        public DateTime? CheckInTime { get; set; }
    }
    public class CheckInRequest
    {
        public string ReferenceNumber { get; set; } // Booking reference number
        public List<string> Passengers { get; set; }

    }
    public class UpdateCheckInRequest
    {
        public string CheckInId { get; set; }
        public string SeatNumber { get; set; }
    }
    public class UpdateBookingSeats
    {
        public string ReferenceNumber { get; set; }
        public List<PassengerSeat> SeatAssignments { get; set; }
    }

    public class PassengerSeat
    {
        public string CheckInId { get; set; }
        public string PassengerName { get; set; }
        public string SeatNumber { get; set; }
    }
}
