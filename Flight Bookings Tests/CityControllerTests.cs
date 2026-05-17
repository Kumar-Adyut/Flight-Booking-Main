using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityService;
using CityService.Controllers;
using CityService.Process;
using CityService.Repository;
using CommonUse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Flight_Bookings_Tests
{
    [TestClass]
    public sealed class CityControllerTests
    {
        private Mock<ICity> _mockRepo;
        private CityProcess _cityProcess;
        private CityController _controller;
        private Mock<ILogger<CityController>> _mockLogger;

        [TestInitialize]
        public void Setup()
        {
            // Mock the ICity repository
            _mockRepo = new Mock<ICity>();

            // Mock the ILogger<CityController>
            _mockLogger = new Mock<ILogger<CityController>>();

            // Pass the mocked ICity repository and ILogger to CityProcess
            _cityProcess = new CityProcess(_mockRepo.Object);

            // Initialize the controller with the CityProcess and mocked ILogger<CityController>
            _controller = new CityController(_cityProcess, _mockLogger.Object);
        }

        // GetAllData
        [TestMethod]
        public async Task GetAllData_ShouldReturnOkResult_WhenCitiesExist()
        {
            // Arrange
            var cities = new List<City> { new City { CityCode = "DEL", CityName = "Delhi" }, new City { CityCode = "MUM", CityName = "Mumbai" } };
            _mockRepo.Setup(p => p.GetAllDataAsync()).ReturnsAsync(cities);

            // Act
            var result = await _controller.GetAllData();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // Status code 200 for Ok
        }
        [TestMethod]
        public async Task GetAllData_ShouldReturnNotFound_WhenExceptionOccurs()
        {
            // Arrange
            _mockRepo.Setup(p => p.GetAllDataAsync()).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.GetAllData();

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); // Status code 404 for Not Found
        }


        //Add City
        [TestMethod]
        public async Task AddCity_ShouldReturnOkResult_WhenCityIsAdded()
        {
            // Arrange
            var city = new City { CityCode = "KOL", CityName = "Kolkata" };
            _mockRepo.Setup(p => p.AddCityAsync(It.IsAny<City>())).ReturnsAsync(true);

            // Act
            var result = await _controller.AddCity(city);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task AddCity_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var city = new City { CityCode = "KOL", CityName = "Kolkata" };
            _mockRepo.Setup(p => p.AddCityAsync(It.IsAny<City>())).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.AddCity(city);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode); // Status code 400 for BadRequest
        }

        // Update City
        [TestMethod]
        public async Task UpdateCity_ShouldReturnOkResult_WhenCityIsUpdated()
        {
            // Arrange
            var cityCode = "IXR";
            var city = new City { CityCode = "IXR", CityName = "Ranchi City", AirportCharge = 50 };

            // Mock the repository to return a valid city when searching by CityCode
            _mockRepo.Setup(p => p.GetCityByCityCodeAsync(cityCode)).ReturnsAsync(new City
            {
                CityCode = "IXR",
                CityName = "Ranchi",
                AirportCharge = 40,
                IsActive = true
            });

            // Mock the repository to return the updated city when UpdateCityAsync is called
            _mockRepo.Setup(p => p.UpdateCityAsync(cityCode, city)).ReturnsAsync(city);

            // Act
            var result = await _controller.UpdateCity(cityCode, city);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // Status code 200 for Ok
            var returnedCity = okResult.Value as City;
            Assert.AreEqual("Ranchi City", returnedCity.CityName); // Assert that the city name is updated
        }

        [TestMethod]
        public async Task UpdateCity_ShouldReturnNotFound_WhenCityNotFound()
        {
            // Arrange
            var cityCode = "XYZ";
            var city = new City { CityCode = "XYZ", CityName = "UnknownCity" };
            _mockRepo.Setup(p => p.UpdateCityAsync(cityCode, city)).ThrowsAsync(new KeyNotExistException("City not found"));

            // Act
            var result = await _controller.UpdateCity(cityCode, city);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); // Status code 404 for NotFound
        }



        // GetByCityCode
        [TestMethod]
        public async Task GetCityByCityCode_ShouldReturnOkResult_WhenCityExists()
        {
            // Arrange
            var cityCode = "DEL";
            var city = new City { CityCode = "DEL", CityName = "Delhi" };
            _mockRepo.Setup(p => p.GetCityByCityCodeAsync(cityCode)).ReturnsAsync(city);

            // Act
            var result = await _controller.GetCityByCode(cityCode);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // Status code 200 for Ok
        }
        [TestMethod]
        public async Task GetCityByCityCode_ShouldReturnNotFound_WhenCityDoesNotExist()
        {
            // Arrange
            var cityCode = "XYZ";
            _mockRepo.Setup(p => p.GetCityByCityCodeAsync(cityCode)).ThrowsAsync(new KeyNotExistException("City not found"));

            // Act
            var result = await _controller.GetCityByCode(cityCode);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); // Status code 404 for NotFound
        }


        // Delete City
        [TestMethod]
        public async Task DeleteCity_ShouldReturnOkResult_WhenCityIsDeleted()
        {
            // Arrange
            var cityCode = "DEL";
            _mockRepo.Setup(p => p.DeleteCityAsync(cityCode)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCity(cityCode);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // Status code 200 for Ok
        }
        [TestMethod]
        public async Task DeleteCity_ShouldReturnNotFound_WhenDeleteFails()
        { 
	        // Arrange
	        var cityCode = "XYZ";
            _mockRepo.Setup(p => p.DeleteCityAsync(cityCode)).ThrowsAsync(new KeyNotExistException("City not found"));

            // Act
            var result = await _controller.DeleteCity(cityCode);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); // Status code 404 for NotFound
        }
        

        

        // Get City By City Name

        [TestMethod]
        public async Task GetCityByCityName_ShouldReturnOkResult_WhenCityExists()
        {
            // Arrange
            var cityName = "Delhi";
            var city = new City { CityCode = "DEL", CityName = "Delhi" };
            _mockRepo.Setup(p => p.GetCityByNameAsync(cityName)).ReturnsAsync(city);

            // Act
            var result = await _controller.GetCityByName(cityName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // Status code 200 for Ok
        }

        [TestMethod]
        public async Task GetCityByCityName_ShouldReturnNotFound_WhenCityDoesNotExist()
        {
            // Arrange
            var cityName = "xyzser";
            _mockRepo.Setup(p => p.GetCityByNameAsync(cityName)).ThrowsAsync(new KeyNotExistException("City not found"));

            // Act
            var result = await _controller.GetCityByName(cityName);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); // Status code 404 for NotFound
        }
        
        
        
        
        // Update Airport Charges
        [TestMethod]
        public async Task UpdateAirportCharge_ShouldReturnOkResult_WhenAirportChargeIsUpdated()
        {
            // Arrange
            var cityCode = "DEL";
            int airportCharge = 500;
            var updatedCity = new City { CityCode = "DEL", CityName = "Delhi", AirportCharge = airportCharge };
            _mockRepo.Setup(p => p.UpdateAirportChargeAsync(cityCode, airportCharge)).ReturnsAsync(updatedCity);

            // Act
            var result = await _controller.UpdateAirportCharge(cityCode, airportCharge);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // Status code 200 for Ok
        }
        [TestMethod]
        public async Task UpdateAirportCharge_ShouldReturnNotFound_WhenExceptionOccurs()
        {
            // Arrange
            var cityCode = "XYZ";
            int airportCharge = 500;
            _mockRepo.Setup(p => p.UpdateAirportChargeAsync(cityCode, airportCharge)).ThrowsAsync(new KeyNotExistException("Some error"));

            // Act
            var result = await _controller.UpdateAirportCharge(cityCode, airportCharge);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); // Status code 404 for NotFound
        }
        



    }
}
