namespace FareService.Repository
{
    public interface IFare
    {
        Task<bool> AddFareForFlight(Fare fare);
        Task<bool> RemoveFareForFlight(int fareId);
        Task<Fare> UpdateFareByFareId(int fareId, decimal basePrice, decimal convenienceFee);
        Task<Fare> UpdateFareByFlightId(int flightId, decimal basePrice, decimal convenienceFee);
        Task<decimal> GetFareByFlightId(int flightId);
        Task<decimal> GetFareByFareID(int fareId);

        Task<decimal> GetTotalFare(int fareId);
    }
}
