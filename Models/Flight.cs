using System;
using System.Collections.Generic;

namespace FlightService;

public partial class Flight
{
    public int FlightId { get; set; }

    public string FlightNo { get; set; } = null!;

    public string FromCity { get; set; } = null!;

    public string ToCity { get; set; } = null!;

    public DateOnly DepartureDate { get; set; }

    public TimeOnly DepartureTime { get; set; }

    public TimeOnly ArrivalTime { get; set; }

    public int AvailableSeats { get; set; }

    public bool IsActive { get; set; }
}
