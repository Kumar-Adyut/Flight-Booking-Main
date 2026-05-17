
using System.Net.Http;
using CommonUse;
using Microsoft.EntityFrameworkCore;
using Models;

namespace CheckInService.Repository
{
    public class CheckInRepository : ICheckIn
    {
        CheckInDbContext context;
        public CheckInRepository(CheckInDbContext context)
        {
            this.context = context;
        }
        public async Task<List<CheckInResponse>> CheckInAsync(CheckInRequest request)
        {
            // Generate a unique Check-in ID
            //string checkInId = "CHK" + new Random().Next(10000, 99999);
            //var seatNumber = GenerateSeatNumber();
            //var checkIn = new CheckIn
            //{
            //    CheckInId = checkInId,
            //    ReferenceNumber = request.ReferenceNumber,
            //    CheckInTime = DateTime.Now,
            //    SeatNumber = seatNumber
            //};

            //context.CheckIns.Add(checkIn);
            //await context.SaveChangesAsync();



            //return new CheckInResponse
            //{
            //    CheckInId = checkIn.CheckInId,
            //    ReferenceNumber = checkIn.ReferenceNumber,

            //    SeatNumber = checkIn.SeatNumber,
            //    CheckInTime = checkIn.CheckInTime
            //};


            var seatNumbers = GenerateUniqueSeatNumbers(request.Passengers.Count);
            var checkIns = new List<CheckIn>();
            //string checkInId = "CHK" + new Random().Next(10000, 99999);
            for (int i = 0; i < request.Passengers.Count; i++)
            {
                var checkIn = new CheckIn
                {
                    CheckInId = "CHK" + new Random().Next(1000, 9999),
                    ReferenceNumber = request.ReferenceNumber,
                    PassengerName = request.Passengers[i],
                    SeatNumber = seatNumbers[i]
                };

                checkIns.Add(checkIn);
            }

            await context.CheckIns.AddRangeAsync(checkIns);
            await context.SaveChangesAsync();

            var checkInResponses = checkIns.Select(c => new CheckInResponse
            {
                CheckInId = c.CheckInId,
                ReferenceNumber = c.ReferenceNumber,
                PassengerName = c.PassengerName,
                SeatNumber = c.SeatNumber,
                CheckInTime = c.CheckInTime
            }).ToList();

            return checkInResponses;
        }

        

        public async Task<CheckInResponse> GetCheckInDetailsAsync(string checkInId)
        {
            var checkIn = await context.CheckIns.FirstOrDefaultAsync(c => c.CheckInId == checkInId);
            if (checkIn == null) throw new Exception("Check-in not found.");

            return new CheckInResponse
            {
                CheckInId = checkIn.CheckInId,
                ReferenceNumber = checkIn.ReferenceNumber,
                PassengerName = checkIn.PassengerName,
                SeatNumber = checkIn.SeatNumber,
                CheckInTime = checkIn.CheckInTime
            };
        }

        private List<string> GenerateUniqueSeatNumbers(int count)
        {
            HashSet<string> seatNumbers = new HashSet<string>();
            Random random = new Random();

            while (seatNumbers.Count < count)
            {
                int seat = random.Next(1, 30);
                char row = (char)('A' + random.Next(0, 6));
                seatNumbers.Add($"{row}{seat}");
            }

            return seatNumbers.ToList();
        }
    }
}
