using System;
using System.Collections.Generic;

namespace Models;

public partial class Passenger
{
    public int PassengerId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? SeatNumber { get; set; }

    public bool IsActive { get; set; }

    public string Gender { get; set; } = null!;

    public string? CheckInId { get; set; }

    public string ReferenceNumber { get; set; } = null!;

    public virtual Booking ReferenceNumberNavigation { get; set; } = null!;
}
