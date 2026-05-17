using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FareService;
using FareService.Controllers;
using FareService.Process;
using FareService.Repository;
using FlightService.Controllers;
using FlightService.Process;
using FlightService.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Flight_Bookings_Tests
{
    [TestClass]
    public sealed class FareControllerTests
    {
        
        private Mock<IFare> _mockRepo;
        private FareProcess _fareProcess;
        private FareController _controller;
        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IFare>();
            _fareProcess=new FareProcess(_mockRepo.Object);
            _controller=new FareController(_fareProcess);
        }
        [TestMethod]
        public async Task AddFareForFlight_ShouldReturnOk_WhenFareAddedSuccessfully()
        {
            // Arrange
            var fare = new Fare { FlightId = 1, BasePrice = 100.00m }; // Example fare data
            _mockRepo.Setup(p => p.AddFareForFlight(fare)).ReturnsAsync(true); // Mock the process layer

            // Act
            var result = await _controller.AddFareForFlight(fare);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Fare added successfully.", okResult.Value);
        }

        [TestMethod]
        public async Task AddFareForFlight_ShouldReturnBadRequest_WhenAddFareFails()
        {
            // Arrange
            var fare = new Fare { FlightId = 1, BasePrice = 100.00m };
            _mockRepo.Setup(p => p.AddFareForFlight(fare)).ReturnsAsync(false); // Mock the failure scenario

            // Act
            var result = await _controller.AddFareForFlight(fare);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Failed to add fare.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddFareForFlight_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var fare = new Fare { FlightId = 1, BasePrice = 100.00m };
            _mockRepo.Setup(p => p.AddFareForFlight(fare)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.AddFareForFlight(fare);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));
        }
        [TestMethod]
        public async Task RemoveFareForFlight_ShouldReturnOk_WhenFareRemovedSuccessfully()
        {
            // Arrange
            int fareId = 1;
            _mockRepo.Setup(p => p.RemoveFareForFlight(fareId)).ReturnsAsync(true); // Simulate fare removal success

            // Act
            var result = await _controller.RemoveFareForFlight(fareId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Fare removed successfully.", okResult.Value);
        }

        [TestMethod]
        public async Task RemoveFareForFlight_ShouldReturnNotFound_WhenFareNotExist()
        {
            // Arrange
            int fareId = 1;
            _mockRepo.Setup(p => p.RemoveFareForFlight(fareId)).ReturnsAsync(false); // Simulate fare not found

            // Act
            var result = await _controller.RemoveFareForFlight(fareId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Fare not found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task RemoveFareForFlight_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int fareId = 1;
            _mockRepo.Setup(p => p.RemoveFareForFlight(fareId)).ThrowsAsync(new Exception("Database error")); // Simulate exception

            // Act
            var result = await _controller.RemoveFareForFlight(fareId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));
        }
        [TestMethod]
        public async Task UpdateFareByFareId_ShouldReturnOk_WhenFareUpdatedSuccessfully()
        {
            // Arrange
            var fareId = 1;
            var fareToUpdate = new Fare { FareId = fareId, BasePrice = 500, ConvenienceFee = 50 };
            _mockRepo.Setup(p => p.UpdateFareByFareId(fareId, fareToUpdate.BasePrice, fareToUpdate.ConvenienceFee))
                        .ReturnsAsync(fareToUpdate);  // Simulate successful update

            // Act
            var result = await _controller.UpdateFareByFareId(fareId, fareToUpdate);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            var updatedFare = okResult.Value as Fare;
            Assert.AreEqual(fareId, updatedFare.FareId);  // Ensure correct fare is returned
            Assert.AreEqual(500, updatedFare.BasePrice);  // Assert that the base price was updated
            Assert.AreEqual(50, updatedFare.ConvenienceFee);  // Assert that the convenience fee was updated
        }

        [TestMethod]
        public async Task UpdateFareByFareId_ShouldReturnNotFound_WhenFareNotFound()
        {
            // Arrange
            var fareId = 1;
            var fareToUpdate = new Fare { FareId = fareId, BasePrice = 500, ConvenienceFee = 50 };
            _mockRepo.Setup(p => p.UpdateFareByFareId(fareId, fareToUpdate.BasePrice, fareToUpdate.ConvenienceFee))
                        .ReturnsAsync((Fare)null);  // Simulate fare not found

            // Act
            var result = await _controller.UpdateFareByFareId(fareId, fareToUpdate);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Fare not found.", notFoundResult.Value);  // Assert that the not found message is returned
        }

        [TestMethod]
        public async Task UpdateFareByFareId_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var fareId = 1;
            var fareToUpdate = new Fare { FareId = fareId, BasePrice = 500, ConvenienceFee = 50 };
            _mockRepo.Setup(p => p.UpdateFareByFareId(fareId, fareToUpdate.BasePrice, fareToUpdate.ConvenienceFee))
                        .ThrowsAsync(new Exception("Database error"));  // Simulate exception

            // Act
            var result = await _controller.UpdateFareByFareId(fareId, fareToUpdate);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));  // Assert error message is returned
        }
        [TestMethod]
        public async Task UpdateFareByFlightId_ShouldReturnOk_WhenFareUpdatedSuccessfully()
        {
            // Arrange
            var flightId = 1;
            var fareToUpdate = new Fare { FlightId = flightId, BasePrice = 500, ConvenienceFee = 50 };
            _mockRepo.Setup(p => p.UpdateFareByFlightId(flightId, fareToUpdate.BasePrice, fareToUpdate.ConvenienceFee))
                        .ReturnsAsync(fareToUpdate);  // Simulate successful update

            // Act
            var result = await _controller.UpdateFareByFlightId(flightId, fareToUpdate);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(okResult.Value);
            var updatedFare = okResult.Value as Fare;
            Assert.AreEqual(flightId, updatedFare.FlightId);  // Ensure correct fare is returned
            Assert.AreEqual(500, updatedFare.BasePrice);  // Assert that the base price was updated
            Assert.AreEqual(50, updatedFare.ConvenienceFee);  // Assert that the convenience fee was updated
        }

        [TestMethod]
        public async Task UpdateFareByFlightId_ShouldReturnNotFound_WhenFareNotFound()
        {
            // Arrange
            var flightId = 1;
            var fareToUpdate = new Fare { FlightId = flightId, BasePrice = 500, ConvenienceFee = 50 };
            _mockRepo.Setup(p => p.UpdateFareByFlightId(flightId, fareToUpdate.BasePrice, fareToUpdate.ConvenienceFee))
                        .ReturnsAsync((Fare)null);  // Simulate fare not found

            // Act
            var result = await _controller.UpdateFareByFlightId(flightId, fareToUpdate);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Fare not found.", notFoundResult.Value);  // Assert that the not found message is returned
        }

        [TestMethod]
        public async Task UpdateFareByFlightId_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var flightId = 1;
            var fareToUpdate = new Fare { FlightId = flightId, BasePrice = 500, ConvenienceFee = 50 };
            _mockRepo.Setup(p => p.UpdateFareByFlightId(flightId, fareToUpdate.BasePrice, fareToUpdate.ConvenienceFee))
                        .ThrowsAsync(new Exception("Database error"));  // Simulate exception

            // Act
            var result = await _controller.UpdateFareByFlightId(flightId, fareToUpdate);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));  // Assert error message is returned
        }
        [TestMethod]
        public async Task GetFareByFlightId_ShouldReturnFare_WhenFareExists()
        {
            // Arrange
            int flightId = 1;
            decimal expectedFare = 150.50m;

            // Mock the repository method to return a fare value
            _mockRepo.Setup(repo => repo.GetFareByFlightId(flightId)).ReturnsAsync(expectedFare);

            // Act
            var result = await _controller.GetFareByFlightId(flightId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedFare, okResult.Value);
        }

        [TestMethod]
        public async Task GetFareByFlightId_ShouldReturnNotFound_WhenFareDoesNotExist()
        {
            // Arrange
            int flightId = 1;
            decimal fare = 0; // Assume 0 means no fare found

            // Mock the repository method to return a fare of 0 (not found)
            _mockRepo.Setup(repo => repo.GetFareByFlightId(flightId)).ReturnsAsync(fare);

            // Act
            var result = await _controller.GetFareByFlightId(flightId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Fare not found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetFareByFlightId_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var flightId = 1;
            _mockRepo.Setup(p => p.GetFareByFlightId(flightId)).ThrowsAsync(new Exception("Database error"));  // Simulate exception

            // Act
            var result = await _controller.GetFareByFlightId(flightId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));  // Assert error message is returned
        }

        [TestMethod]
        public async Task GetFareByFareID_ShouldReturnOk_WhenFareExists()
        {
            // Arrange
            var fareId = 1;
            decimal expectedFare = 150.50m;
            _mockRepo.Setup(p => p.GetFareByFareID(fareId)).ReturnsAsync(expectedFare); // Simulate fare found

            // Act
            var result = await _controller.GetFareByFareID(fareId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedFare, okResult.Value);
        }

        [TestMethod]
        public async Task GetFareByFareID_ShouldReturnNotFound_WhenFareDoesNotExist()
        {
            // Arrange
            var fareId = 1;
            decimal fare = 0;
            _mockRepo.Setup(p => p.GetFareByFareID(fareId)).ReturnsAsync(fare); // Simulate fare not found

            // Act
            var result = await _controller.GetFareByFareID(fareId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Fare not found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetFareByFareID_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var fareId = 1;
            _mockRepo.Setup(p => p.GetFareByFareID(fareId)).ThrowsAsync(new Exception("Database error"));  // Simulate exception

            // Act
            var result = await _controller.GetFareByFareID(fareId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);  // Assert that the status code is InternalServerError (500)
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));  // Assert that the error message is returned
        }
        [TestMethod]
        public async Task GetTotalFare_ShouldReturnOk_WhenFareExists()
        {
            // Arrange
            var fareId = 1;
            var fare = new Fare { FareId = fareId, BasePrice = 500, ConvenienceFee = 50 };
            var expectedTotalFare = fare.BasePrice + fare.ConvenienceFee;
            _mockRepo.Setup(p => p.GetTotalFare(fareId)).ReturnsAsync(expectedTotalFare);  // Simulate successful calculation

            // Act
            var result = await _controller.GetTotalFare(fareId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);  // Assert that the status code is OK (200)
            Assert.AreEqual(expectedTotalFare, okResult.Value);  // Assert the returned total fare is correct
        }

        [TestMethod]
        public async Task GetTotalFare_ShouldReturnNotFound_WhenFareDoesNotExist()
        {
            // Arrange
            var fareId = 1;
            _mockRepo.Setup(p => p.GetTotalFare(fareId)).ThrowsAsync(new Exception("Fare not found"));  // Simulate fare not found

            // Act
            var result = await _controller.GetTotalFare(fareId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);  // Assert that the status code is Internal Server Error (500)
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));  // Assert that error message is correct
        }

        [TestMethod]
        public async Task GetTotalFare_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var fareId = 1;
            _mockRepo.Setup(p => p.GetTotalFare(fareId)).ThrowsAsync(new Exception("An unexpected error occurred"));  // Simulate exception

            // Act
            var result = await _controller.GetTotalFare(fareId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);  // Assert that the status code is InternalServerError (500)
            Assert.IsTrue(((string)statusCodeResult.Value).Contains("Internal server error"));  // Assert that the error message is returned
        }
    }
}
