using FareService.Repository;

namespace FareService.Process
{
    public class FareProcess
    {
        private readonly IFare _repository;
        public FareProcess(IFare repository)
        {
            _repository = repository;
        }
        public async Task<bool> AddFare(Fare fare)
        {
            return await _repository.AddFareForFlight(fare);
        }
        public async Task<bool> RemoveFare(int fareId)
        {
            return await _repository.RemoveFareForFlight(fareId);
        }
        public async Task<Fare> UpdateFareByFareId(int fareId, decimal basePrice, decimal convenienceFee)
        {
            return await _repository.UpdateFareByFareId(fareId, basePrice, convenienceFee);
        }
        public async Task<Fare> UpdateFareByFlightId(int flightId, decimal basePrice, decimal convenienceFee)
        {
            return await _repository.UpdateFareByFlightId(flightId, basePrice, convenienceFee);
        }
        public async Task<decimal> GetFareByFlightId(int flightId)
        {
            return await _repository.GetFareByFlightId(flightId);
        }
        public async Task<decimal> GetFareByFareID(int fareId)
        {
            return await _repository.GetFareByFareID(fareId);
        }
        public async Task<decimal> GetTotalFare(int fareId)
        {
            return await _repository.GetTotalFare(fareId);
        }
    }
}
