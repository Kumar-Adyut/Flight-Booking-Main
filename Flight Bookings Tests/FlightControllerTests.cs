using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityService.Controllers;
using CityService.Process;
using CityService.Repository;
using CommonUse;
using FlightService;
using FlightService.Controllers;
using FlightService.Process;
using FlightService.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Flight_Bookings_Tests
{
    [TestClass]
    public sealed class FlightControllerTests
    {

        private Mock<IFlight> _mockRepo;
        private FlightProcess _flightProcess;
        private FlightController _controller;
        

        [TestInitialize]
        public void Setup()
        {
            // Mock the IFlight repository
            _mockRepo = new Mock<IFlight>();

            // Pass the mocked IFlight repository
            _flightProcess = new FlightProcess(_mockRepo.Object);

            // Initialize the controller with the FlightProcess
            _controller = new FlightController(_flightProcess);
        }
        // Add Flights
        [TestMethod]
        public async Task AddFlight_ShouldReturnOk_WhenFlightIsAddedSuccessfully()
        {
            // Arrange
            var flight = new Flight { FlightId = 101, FlightNo = "Flight101" };  // Sample data
            _mockRepo.Setup(p => p.AddFlight(flight)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddFlight(flight);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));  // Check if it's OkObjectResult
            var okResult = result as OkObjectResult;  // Now safely cast the result
            Assert.AreEqual("Flight Added Successfully", okResult?.Value);  // Assert the value inside OkObjectResult
        }

        [TestMethod]
        public async Task AddFlight_ShouldReturnNotFound_WhenFlightAlreadyExists()
        {
            // Arrange
            var flight = new Flight { FlightId = 1, FlightNo = "Flight101" };  // Sample data
            _mockRepo.Setup(p => p.AddFlight(flight)).ThrowsAsync(new KeyAlreadyExistsException("Id Already Exists"));

            // Act
            var result = await _controller.AddFlight(flight);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Id Already Exists", notFoundResult.Value);
        }

        [TestMethod]
        public async Task AddFlight_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var flight = new Flight { FlightId = 1, FlightNo = "Flight 101" };  // Sample data
            _mockRepo.Setup(p => p.AddFlight(flight)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.AddFlight(flight);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Something went wrong", badRequestResult.Value);
        }
        // GetFlightByDepartureDate
        [TestMethod]
        public async Task GetFlightByDepartureDate_ShouldReturnOk_WhenFlightsExist()
        {
            // Arrange
            var flightList = new List<Flight>
            {
                new Flight { FlightId = 1, FromCity = "Delhi", ToCity = "Ranchi", DepartureDate = new DateOnly(2025, 3, 10) }
            };
            _mockRepo.Setup(p => p.GetFlightByDepartureDate("Delhi", "Ranchi", new DateOnly(2025, 3, 10)))
                        .ReturnsAsync(flightList);

            // Act
            var result = await _controller.GetFlightByDepartureDate("Delhi", "Ranchi", new DateOnly(2025, 3, 10));

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;  // Cast to OkObjectResult
            Assert.IsNotNull(okResult);  // Assert that it's not null
            Assert.AreEqual(200, okResult.StatusCode);  // Assert status code
            var returnedFlights = okResult.Value as List<Flight>;  // Cast Value to List<Flight>
            Assert.IsNotNull(returnedFlights);  // Ensure flights list is not null
            Assert.AreEqual(1, returnedFlights.Count);  // Assert the number of flights
            Assert.AreEqual("Delhi", returnedFlights[0].FromCity);  // Check first flight details
            Assert.AreEqual("Ranchi", returnedFlights[0].ToCity);
        }

        [TestMethod]
        public async Task GetFlightByDepartureDate_ShouldReturnNotFound_WhenNoFlightsExist()
        {
            // Arrange
            _mockRepo.Setup(p => p.GetFlightByDepartureDate("Ranchi", "Kochi", new DateOnly(2025, 3, 10)))
                        .ThrowsAsync(new KeyNotExistException("Flights not exist"));

            // Act
            var result = await _controller.GetFlightByDepartureDate("Ranchi", "Kochi", new DateOnly(2025, 3, 10));

            //// Assert
            //var notFoundResult = Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            //Assert.AreEqual("Flights not exist", (notFoundResult as NotFoundObjectResult).Value);
            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;  // Cast to OkObjectResult
            Assert.IsNotNull(notFoundResult);  // Assert that it's not null
            Assert.AreEqual(404, notFoundResult.StatusCode);  // Assert status code
        }

        //[TestMethod]
        //public async Task GetFlightByDepartureDate_ShouldReturnBadRequest_WhenExceptionOccurs()
        //{
        //    // Arrange
        //    _mockRepo.Setup(p => p.GetFlightByDepartureDate("New York", "London", new DateOnly(2025, 3, 10)))
        //                .ThrowsAsync(new Exception("An unexpected error occurred"));

        //    // Act
        //    var result = await _controller.GetFlightByDepartureDate("New York", "London", new DateOnly(2025, 3, 10));

        //    // Assert
        //    var badRequestResult = Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        //    Assert.AreEqual("An unexpected error occurred", (badRequestResult as BadRequestObjectResult).Value);
        //}

        //[TestMethod]
        //public async Task GetFlightByDepartureDate_ShouldReturnBadRequest_WhenFromOrToCityIsEmpty()
        //{
        //    // Arrange
        //    _mockRepo.Setup(p => p.GetFlightByDepartureDate("", "London", new DateOnly(2025, 3, 10)))
        //                .ThrowsAsync(new ArgumentException("Data can't be null"));

        //    // Act
        //    var result = await _controller.GetFlightByDepartureDate("", "London", new DateOnly(2025, 3, 10));

        //    // Assert
        //    var badRequestResult = Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        //    Assert.AreEqual("Data can't be null", (badRequestResult as BadRequestObjectResult).Value);
        //}
        //

        //GetFlightById
        [TestMethod]
        public async Task GetFlightById_ShouldReturnOk_WhenFlightExists()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight { FlightId = flightId, FromCity = "Delhi", ToCity = "Ranchi" };

            // Mock the Process method to return the flight
            _mockRepo.Setup(p => p.GetFlightById(flightId)).ReturnsAsync(flight);

            // Act
            var result = await _controller.GetFlightById(flightId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));  // Check that it returns OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);  // Assert it's not null
            Assert.AreEqual(200, okResult.StatusCode);  // Assert that the status code is OK (200)
            var returnedFlight = okResult.Value as Flight;
            Assert.IsNotNull(returnedFlight);  // Assert that the returned flight is not null
            Assert.AreEqual(flightId, returnedFlight.FlightId);  // Assert that the flightId matches
        }

        [TestMethod]
        public async Task GetFlightById_ShouldReturnNotFound_WhenFlightDoesNotExist()
        {
            // Arrange
            int flightId = 1;

            // Mock the Process method to throw a KeyNotFoundException
            _mockRepo.Setup(p => p.GetFlightById(flightId)).ThrowsAsync(new KeyNotFoundException("Id not found"));

            // Act
            var result = await _controller.GetFlightById(flightId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));  // Check that it returns NotFoundObjectResult
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);  // Assert it's not null
            Assert.AreEqual(404, notFoundResult.StatusCode);  // Assert that the status code is NotFound (404)
            Assert.AreEqual("Id not found", notFoundResult.Value);  // Assert that the error message matches
        }

        [TestMethod]
        public async Task GetFlightById_ShouldReturnBadRequest_WhenAnExceptionOccurs()
        {
            // Arrange
            int flightId = 1;

            // Mock the Process method to throw a generic exception
            _mockRepo.Setup(p => p.GetFlightById(flightId)).ThrowsAsync(new Exception("An error occurred"));

            // Act
            var result = await _controller.GetFlightById(flightId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));  // Check that it returns BadRequestObjectResult
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);  // Assert it's not null
            Assert.AreEqual(400, badRequestResult.StatusCode);  // Assert that the status code is BadRequest (400)
            Assert.AreEqual("An error occurred", badRequestResult.Value);  // Assert that the error message matches
        }

        // Remove Flight
        [TestMethod]
        public async Task RemoveFlight_ShouldReturnOk_WhenFlightDeletedSuccessfully()
        {
            // Arrange
            int flightId = 1;
            _mockRepo.Setup(p => p.RemoveFlight(flightId)).ReturnsAsync(true);  // Simulate successful removal

            // Act
            var result = await _controller.RemoveFlight(flightId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));  // Check that it returns OkObjectResult
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);  // Assert it's not null
            Assert.AreEqual(200, okResult.StatusCode);  // Assert status code is OK (200)
            Assert.AreEqual("Deleted successfully", okResult.Value);  // Assert success message
        }

        [TestMethod]
        public async Task RemoveFlight_ShouldReturnNotFound_WhenFlightDoesNotExist()
        {
            // Arrange
            int flightId = 1;
            _mockRepo.Setup(p => p.RemoveFlight(flightId)).ThrowsAsync(new KeyNotExistException("Id not found"));  // Simulate flight not found

            // Act
            var result = await _controller.RemoveFlight(flightId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));  // Check that it returns NotFoundObjectResult
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);  // Assert it's not null
            Assert.AreEqual(404, notFoundResult.StatusCode);  // Assert status code is NotFound (404)
            Assert.AreEqual("Id not found", notFoundResult.Value);  // Assert the error message is correct
        }

        [TestMethod]
        public async Task RemoveFlight_ShouldReturnBadRequest_WhenAnExceptionOccurs()
        {
            // Arrange
            int flightId = 1;
            _mockRepo.Setup(p => p.RemoveFlight(flightId)).ThrowsAsync(new Exception("An error occurred"));  // Simulate unexpected error

            // Act
            var result = await _controller.RemoveFlight(flightId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));  // Check that it returns BadRequestObjectResult
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);  // Assert it's not null
            Assert.AreEqual(400, badRequestResult.StatusCode);  // Assert status code is BadRequest (400)
            Assert.AreEqual("An error occurred", badRequestResult.Value);  // Assert error message is correct
        }

        //Update Available Seats

        [TestMethod]
        public async Task UpdateAvailableSeat_ShouldReturnOk_WhenFlightUpdatedSuccessfully()
        {
            // Arrange
            int flightId = 1;
            int availableSeats = 50;
            var updatedFlight = new Flight { FlightId = flightId, AvailableSeats = availableSeats, FromCity = "Delhi", ToCity = "Ranchi" };
            _mockRepo.Setup(p => p.UpdateAvailableSeat(flightId, availableSeats)).ReturnsAsync(updatedFlight); // Simulate successful update

            // Act
            var result = await _controller.UpdateAvailableSeat(flightId, availableSeats);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));  // Check that it returns OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);  // Assert it's not null
            Assert.AreEqual(200, okResult.StatusCode);  // Assert status code is OK (200)
            var returnedFlight = okResult.Value as Flight;
            Assert.IsNotNull(returnedFlight);  // Ensure flight object is not null
            Assert.AreEqual(flightId, returnedFlight.FlightId);  // Assert the returned flight ID matches
            Assert.AreEqual(availableSeats, returnedFlight.AvailableSeats);  // Assert the available seats are updated correctly
        }

        [TestMethod]
        public async Task UpdateAvailableSeat_ShouldReturnNotFound_WhenFlightDoesNotExist()
        {
            // Arrange
            int flightId = 1;
            int availableSeats = 50;
            _mockRepo.Setup(p => p.UpdateAvailableSeat(flightId, availableSeats)).ThrowsAsync(new KeyNotFoundException("Id not found"));  // Simulate flight not found

            // Act
            var result = await _controller.UpdateAvailableSeat(flightId, availableSeats);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));  // Check that it returns NotFoundObjectResult
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);  // Assert it's not null
            Assert.AreEqual(404, notFoundResult.StatusCode);  // Assert status code is NotFound (404)
            Assert.AreEqual("Id not found", notFoundResult.Value);  // Assert the error message is correct
        }

        [TestMethod]
        public async Task UpdateAvailableSeat_ShouldReturnBadRequest_WhenFlightIdOrAvailableSeatsAreInvalid()
        {
            // Arrange
            int invalidFlightId = -1;  // Invalid flightId (negative value)
            int availableSeats = 50;
            _mockRepo.Setup(p => p.UpdateAvailableSeat(invalidFlightId, availableSeats)).ThrowsAsync(new ArgumentException("Data can't be null"));  // Simulate invalid input

            // Act
            var result = await _controller.UpdateAvailableSeat(invalidFlightId, availableSeats);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));  // Check that it returns BadRequestObjectResult
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);  // Assert it's not null
            Assert.AreEqual(400, badRequestResult.StatusCode);  // Assert status code is BadRequest (400)
            Assert.AreEqual("Data can't be null", badRequestResult.Value);  // Assert error message is correct
        }


        //Update Flights\

        [TestMethod]
        public async Task UpdateFlightDetails_ShouldReturnOk_WhenFlightUpdatedSuccessfully()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight
            {
                FlightId = flightId,
                FlightNo = "AI101",
                FromCity = "Delhi",
                ToCity = "Ranchi",
                DepartureDate = new DateOnly(2025, 3, 10),
                DepartureTime = new TimeOnly(10,00),
                ArrivalTime = new TimeOnly(12,00),
                AvailableSeats = 50
            };
            var updatedFlight = new Flight { FlightId = flightId, FlightNo = "AI101", AvailableSeats = 50 };
            _mockRepo.Setup(p => p.UpdateFlight(flightId, flight)).ReturnsAsync(updatedFlight);

            // Act
            var result = await _controller.UpdateFlightDetails(flightId, flight);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));  // Check that it returns OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);  // Assert it's not null
            Assert.AreEqual(200, okResult.StatusCode);  // Assert status code is OK (200)
            var returnedFlight = okResult.Value as Flight;
            Assert.IsNotNull(returnedFlight);  // Ensure flight object is not null
            Assert.AreEqual(flightId, returnedFlight.FlightId);  // Assert the returned flight ID matches
            Assert.AreEqual(50, returnedFlight.AvailableSeats);  // Assert the available seats are updated correctly
        }

        [TestMethod]
        public async Task UpdateFlightDetails_ShouldReturnNotFound_WhenFlightDoesNotExist()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight
            {
                FlightId = flightId,
                FlightNo = "AI101",
                FromCity = "Delhi",
                ToCity = "Ranchi",
                DepartureDate = new DateOnly(2025, 3, 10),
                DepartureTime = new TimeOnly(10, 00),
                ArrivalTime = new TimeOnly(12, 00),
                AvailableSeats = 50
            };
            _mockRepo.Setup(p => p.UpdateFlight(flightId, flight)).ThrowsAsync(new KeyNotFoundException("Id not found"));

            // Act
            var result = await _controller.UpdateFlightDetails(flightId, flight);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));  // Check that it returns NotFoundObjectResult
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);  // Assert it's not null
            Assert.AreEqual(404, notFoundResult.StatusCode);  // Assert status code is NotFound (404)
            Assert.AreEqual("Id not found", notFoundResult.Value);  // Assert the error message is correct
        }
        [TestMethod]
        public async Task UpdateFlightDetails_ShouldReturnBadRequest_WhenFlightIsNull()
        {
            // Arrange
            int flightId = 1;
            Flight flight = null;  // Flight is null
            _mockRepo.Setup(p => p.UpdateFlight(flightId, flight)).ThrowsAsync(new ArgumentException("Data can't be null"));

            // Act
            var result = await _controller.UpdateFlightDetails(flightId, flight);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));  // Check that it returns BadRequestObjectResult
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);  // Assert it's not null
            Assert.AreEqual(400, badRequestResult.StatusCode);  // Assert status code is BadRequest (400)
            Assert.AreEqual("Data can't be null", badRequestResult.Value);  // Assert the error message is correct
        }
    }
}
