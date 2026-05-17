using System;
using System.Collections.Generic;

namespace Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public string ReferenceNumber { get; set; } = null!;

    public int FlightId { get; set; }

    public decimal TotalFare { get; set; }

    public DateTime? BookingDate { get; set; }

    public string Status { get; set; } = null!;

    public bool CheckedIn { get; set; }

    public string? CheckInId { get; set; }

    public string? SeatNumber { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
}
