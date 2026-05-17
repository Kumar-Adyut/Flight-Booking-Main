namespace CityService.Repository
{
    public interface ICity
    {
        Task<bool> AddCityAsync(City city);
        Task<City> UpdateCityAsync(string cityCode, City city);
        Task<bool> DeleteCityAsync(string cityCode);
        Task<City> UpdateAirportChargeAsync(string cityCode, int airportCharge);
        Task<IEnumerable<City>> GetAllDataAsync();
        Task<City> GetCityByCityCodeAsync(string cityCode);
        Task<City> GetCityByNameAsync(string cityName);
        Task<List<City>> SearchCitiesAsync(string query);
        //Task<decimal> GetAirportChargeAsync(int flightId);
    }
}
