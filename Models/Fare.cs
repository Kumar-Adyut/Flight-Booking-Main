using System;
using System.Collections.Generic;

namespace FareService;

public partial class Fare
{
    public int FareId { get; set; }

    public int FlightId { get; set; }

    public decimal BasePrice { get; set; }

    public decimal ConvenienceFee { get; set; }
}
