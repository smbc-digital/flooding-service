using System;
using System.Threading.Tasks;
using flooding_service.Controllers;
using flooding_service.Controllers.Models;
using flooding_service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace flooding_service_tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;
        private readonly Mock<IFloodingService> _mockFloodingService = new Mock<IFloodingService>();
        public HomeControllerTests()
        {
            _homeController = new HomeController(_mockFloodingService.Object);
        }
        
        [Fact]
        public async Task Post_ShouldReturnOK()
        {
            // Arrange
            _mockFloodingService
                .Setup(_ => _.CreateCase(It.IsAny<FloodingRequest>()))
                .ReturnsAsync("test ref");

            // Act
            var response = await _homeController.Post(new FloodingRequest());
            var statusResponse = response as OkObjectResult;
            
            // Assert
            Assert.NotNull(statusResponse);
            Assert.Equal(200, statusResponse.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldCreateCase()
        {
            // Act
            await _homeController.Post(new FloodingRequest());

            // Assert
            _mockFloodingService.Verify(_ => _.CreateCase(It.IsAny<FloodingRequest>()), Times.Once);
        }


        [Fact]
        public async Task Post_ShouldReturn500()
        {
            // Arrange
            _mockFloodingService
                .Setup(_ => _.CreateCase(It.IsAny<FloodingRequest>()))
                .Throws(new Exception());

            // Act
            var response = await _homeController.Post(new FloodingRequest());
            var statusResponse = response as StatusCodeResult;

            // Assert
            Assert.Equal(500, statusResponse.StatusCode);
        }
    }
}
