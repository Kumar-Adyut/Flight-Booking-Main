using System;
using System.Collections.Generic;

namespace Models;

public partial class CheckIn
{
    public int Id { get; set; }

    public string CheckInId { get; set; } = null!;

    public string ReferenceNumber { get; set; } = null!;

    public string PassengerName { get; set; } = null!;
    public string SeatNumber { get; set; } = null!;

    public DateTime? CheckInTime { get; set; }
}
