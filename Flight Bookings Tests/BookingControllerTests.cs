using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BookingService.Controllers;
using BookingService.Models;
using BookingService.Process;
using BookingService.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace Flight_Bookings_Tests
{
    [TestClass]
    public class BookingControllerTests
    {
        private Mock<IBooking> _mockBookingRepo;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _mockHttpClient;
        private BookingProcess _bookingProcess;
        private BookingController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Mock the IBooking repository
            _mockBookingRepo = new Mock<IBooking>();

            // Mock the HttpMessageHandler
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Setup HttpClient to use the mocked HttpMessageHandler
            _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            // Mocking your BookingProcess with mocked dependencies
            _bookingProcess = new BookingProcess(_mockBookingRepo.Object, _mockHttpClient, new Mock<IConfiguration>().Object);

            // Create the BookingController using the mocked dependencies
            _controller = new BookingController(_bookingProcess);
        }

        [TestMethod]
        public async Task BookFlight_ShouldReturnReferenceNumber_WhenBookingIsSuccessful()
        {
            // Arrange
            var request = new BookingRequest
            {
                FlightId = 123,
                Passengers = new List<PassengerDTO>
            {
                new PassengerDTO { Name = "Amit Kumar", Email = "amit@example.com", Gender = "Male" }
            }
            };

            decimal fare = 200.0m; // Mock or set the fare value here

            string referenceNumber = "AB20251023"; // The expected reference number

            // Mock the repository method to simulate saving booking
            _mockBookingRepo.Setup(repo => repo.BookFlightAsync(It.IsAny<BookingRequest>(), It.IsAny<decimal>()))
                            .ReturnsAsync(referenceNumber);

            // Mock the fare service response via HttpClient
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(fare.ToString()) // The fare response from the mock service
                });

            // Act
            var result = await _controller.BookFlight(request);

            

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult); // Check that the result is Ok

            // Deserialize the response into a strongly typed object
            var response = okResult.Value as BookingResponse;
            Assert.IsNotNull(response); // Ensure the response is not null

            // Assert the returned properties
            Assert.AreEqual("Booking successful", response.Message); // Assert the success message
            Assert.AreEqual(referenceNumber, response.ReferenceNumber); // Assert the reference number

        }
        [TestMethod]
        public async Task BookFlight_ShouldReturn400_WhenRequestIsInvalid()
        {
            // Arrange
            var request = new BookingRequest
            {
                FlightId = 123,
                Passengers = new List<PassengerDTO>() // No passengers in the request
            };

            // Act
            var result = await _controller.BookFlight(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode); // Should return 400
            var response = badRequestResult.Value as string;
            Assert.IsNotNull(response);
            Assert.AreEqual("Invalid booking request", response); // Custom error message (if applicable)
        }


    }
}
