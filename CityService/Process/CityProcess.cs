using CityService.Repository;
using CommonUse;

namespace CityService.Process
{
    public class CityProcess
    {
        private readonly ICity _repo;

        public CityProcess(ICity repo) => _repo = repo;

        public async Task<IEnumerable<City>> GetAllCities()
        {
            try
            {
                return await _repo.GetAllDataAsync();
            }
            
            catch (Exception ex)
            {
               
                throw new Exception("An unexpected error occurred in the process layer.", ex);
                
            }
        }


        public async Task<bool> AddCity(City city)
        {
            var existingCity = await _repo.GetCityByCityCodeAsync(city.CityCode);
            if (existingCity != null)
            {
                throw new KeyAlreadyExistsException("City with the same code already exists.");
            }

            // Call repository to add the new city
            return await _repo.AddCityAsync(city);
        }

        public async Task<City> UpdateCity(string cityCode, City city)
        {
            //Ensure city data is valid before attempting repository action
            if (string.IsNullOrEmpty(city.CityName) || city.AirportCharge < 0)
            {
                throw new InvalidCityDataException("City Name is required and Airport Charge cannot be negative.");
            }

            var existingCity = await _repo.GetCityByCityCodeAsync(cityCode);
            if (existingCity == null)
            {
                throw new KeyNotExistException($"City with code {cityCode} does not exist.");
            }

            return await _repo.UpdateCityAsync(cityCode, city);
        }


        public async Task<City> GetCityByCityCode(string cityCode)
        {
            var city = await _repo.GetCityByCityCodeAsync(cityCode);

            if (city == null)
            {
                throw new KeyNotExistException($"City with CityCode '{cityCode}' does not exist.");
            }

            return city; // Return the found city
        }

        public async Task<bool> DeleteCity(string cityCode)
        {
            var result = await _repo.DeleteCityAsync(cityCode);

            if (!result)
            {
                throw new Exception("Failed to delete the city. It might already be inactive or doesn't exist.");
            }

            return result;
        }


        public async Task<City> UpdateAirportCharge(string cityCode, int airportCharge)
        {
            // Call repository to update the airport charge and return the updated city
            var updatedCity = await _repo.UpdateAirportChargeAsync(cityCode, airportCharge);

            if (updatedCity == null)
            {
                // If the city was not found or update failed, throw an exception
                throw new KeyNotExistException($"City with CityCode '{cityCode}' not found or update failed.");
            }

            return updatedCity;
        }


        public async Task<City> GetCityByName(string cityName)
        {
            var city = await _repo.GetCityByNameAsync(cityName);

            if (city == null)
            {
                // This will throw a custom exception if city is not found
                throw new KeyNotExistException($"City with name '{cityName}' does not exist.");
            }

            return city;
        }
        public async Task<List<City>> SearchCity(string query)
        {
            try
            {
                return await _repo.SearchCitiesAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching cities: " + ex.Message);
            }


        }
    }

}
