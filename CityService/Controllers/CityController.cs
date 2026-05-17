using System.Diagnostics;
using CityService.Process;
using CityService.Repository;
using CommonUse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    
    public class CityController : ControllerBase
    {
        private readonly CityProcess process;
        private readonly ILogger<CityController> logger;

        public CityController(CityProcess p, ILogger<CityController> log) =>
           (process, logger) = (p, log);
        //public CityController(CityProcess p)
        //{
        //    this.process = p;
        //}


        
        [HttpGet("all")]
        //[CustomAuthentication(Roles = "Admin, User")]
        public async Task<IActionResult> GetAllData()
        {
            try
            {
                var res = await process.GetAllCities();
                return Ok(res);
            }
            //catch (ApplicationException ex)
            //{
            //    // Log and return a 500 Internal Server Error with a detailed message
            //    logger.LogError(ex, "An error occurred while retrieving all cities.");
            //    return StatusCode(500, new { message = "An error occurred while retrieving cities. Please try again later." });
            //}
            catch (Exception ex)
            {
                
                logger.LogError(ex, "Unexpected error occurred.");
                return StatusCode(404, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        //[CustomAuthentication(Roles = "Admin")]
        [HttpPost("addCity")]
        public async Task<IActionResult> AddCity([FromBody] City city)
        {
            try
            {
                var res = await process.AddCity(city);
                logger.LogInformation("Row added successfully");
                return Ok("Added Data Successfully");
            }
            catch (KeyAlreadyExistsException ex)
            {
                // Handle the case where the city already exists and return a more specific response
                logger.LogWarning(ex.Message);
                return Conflict(new { message = ex.Message });  // Return 409 Conflict for duplication error
            }
            catch (Exception ex)
            {
                // Log the general exception and return a BadRequest response
                logger.LogError(ex.Message, ex);
                return BadRequest(new { message = "An error occurred while adding the city. Please try again." });
            }
        }
        //[CustomAuthentication(Roles = "Admin")]
        [HttpPut("updateCity/{cityCode}")]
        public async Task<IActionResult> UpdateCity(string cityCode, City city)
        {
            try
            {
                var updatedCity = await process.UpdateCity(cityCode, city);
                return Ok(updatedCity); // Return 200 OK with the updated city
            }
            catch (KeyNotExistException ex)
            {
                // Handle city not found error, return 404 Not Found
                logger.LogWarning($"City with code {cityCode} not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidCityDataException ex)
            {
                // Handle invalid data provided for city, return 400 Bad Request
                logger.LogWarning($"Invalid data for city update: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                logger.LogError($"Unexpected error updating city: {ex.Message}", ex);
                return StatusCode(500, new { message = "An unexpected error occurred while updating the city." });
            }
        }

        //[CustomAuthentication(Roles = "Admin, User")]
        [HttpGet("getCityByCode/{cityCode}")]
        public async Task<IActionResult> GetCityByCode(string cityCode)
        {
            try
            {
                var city = await process.GetCityByCityCode(cityCode);
                return Ok(city);
            }
            catch (KeyNotExistException ex)
            {
                logger.LogWarning($"City not found with CityCode '{cityCode}': {ex.Message}");
                return NotFound(new { message = ex.Message }); // Return 404 if city doesn't exist
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving city by code '{cityCode}': {ex.Message}", ex);
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the city." }); // Return 500 for general errors
            }
        }
       //[CustomAuthentication(Roles = "Admin")]
        [HttpDelete("deleteCity/{cityCode}")]
        public async Task<IActionResult> DeleteCity(string cityCode)
        {
            try
            {
                var success = await process.DeleteCity(cityCode);
                return success ? Ok("City deleted successfully.") : BadRequest("City deletion failed.");
            }
            catch (KeyNotExistException ex)
            {
                // Log a warning when the city doesn't exist
                logger.LogWarning($"City with CityCode '{cityCode}' not found: {ex.Message}");
                return NotFound(new { message = ex.Message });  // Return 404 for a non-existent city
            }
            catch (Exception ex)
            {
                // Log unexpected errors and return a generic 500 status code
                logger.LogError($"Error deleting city with CityCode '{cityCode}': {ex.Message}", ex);
                return StatusCode(500, new { message = "An error occurred while attempting to delete the city." });
            }
        }
        //[CustomAuthentication(Roles = "Admin")]
        [HttpPatch("updateAirportCharge/{cityCode}/{airportCharge}")]
        public async Task<IActionResult> UpdateAirportCharge(string cityCode, int airportCharge)
        {
            try
            {
                var updatedCity = await process.UpdateAirportCharge(cityCode, airportCharge);
                return Ok(updatedCity);
            }
            catch (KeyNotExistException ex)
            {
                // Log specific error for missing city and return 404
                logger.LogWarning($"City with CityCode '{cityCode}' not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log unexpected errors and return 500
                logger.LogError($"Error updating airport charge for CityCode '{cityCode}': {ex.Message}", ex);
                return StatusCode(500, new { message = "An unexpected error occurred while updating the airport charge." });
            }
        }

        //// New methods for additional functionality
        //[HttpGet("getCitiesByCountry/{country}")]
        //public async Task<IActionResult> GetCitiesByCountry(string country)
        //{
        //    try
        //    {
        //        var cities = await process.GetCitiesByCountry(country);
        //        return Ok(cities);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        logger.LogWarning(ex.Message);
        //        return NotFound(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.Message, ex);
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[CustomAuthentication(Roles = "Admin, User")]
        [HttpGet("getCityByName/{cityName}")]
        public async Task<IActionResult> GetCityByName(string cityName)
        {
            try
            {
                var city = await process.GetCityByName(cityName);
                if (city == null)
                {
                    return NotFound(new { message = $"City with name '{cityName}' not found." });
                }

                return Ok(city);
            }
            catch (KeyNotExistException ex)
            {
                logger.LogWarning($"City not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError($"Unexpected error: {ex.Message}", ex);
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        // SEARCH CITIES (BY NAME OR CODE)
        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchCities(string query)
        {
            try
            {
                var cities = await process.SearchCity(query);
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }


}

