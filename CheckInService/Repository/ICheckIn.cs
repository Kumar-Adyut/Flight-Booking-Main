

using CommonUse;

namespace CheckInService.Repository
{
    public interface ICheckIn
    {
        Task<List<CheckInResponse>> CheckInAsync(CheckInRequest request);
        Task<CheckInResponse> GetCheckInDetailsAsync(string referenceNumber);
    }
}
