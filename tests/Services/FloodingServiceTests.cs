using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Models;
using flooding_service.Services;
using Moq;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Gateways.Response;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.ContactDetails;
using StockportGovUK.NetStandard.Models.Mail;
using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using Xunit;

namespace flooding_service_tests.Services
{
    public class FloodingServiceTests
    {
        private readonly FloodingService _floodingService;
        private readonly Mock<IVerintServiceGateway> _mockVerintServiceGateway = new Mock<IVerintServiceGateway>();
        private readonly Mock<IMailingServiceGateway> _mockMailingServiceGateway = new Mock<IMailingServiceGateway>();
        private readonly FloodingRequest _floodingRequest = new FloodingRequest
        {
            HowWouldYouLikeToBeContacted = "phone",
            IsTheFloodingBlockingTheWholePavementOrCausing = "yes",
            Map = new Map
            {
                Lat = "50.23",
                Lng = "-2.255",
                Street = "street, place, 1234"
            },
            Reporter = new ContactDetails
            {
                FirstName = "firstName",
                LastName = "lastName",
                EmailAddress = "test@test.com",
                PhoneNumber = "01222333333"
            },
            TellUsABoutTheFlood = "it is a flood",
            WhatDoYouWantToReport = "a flood",
            WhereIsTheFlood = "pavement",
            WhereIsTheFloodingComingFrom = "river"
        };

        public FloodingServiceTests()
        {
            var mockConfirmAttributeFormOptions = new ConfirmAttributeFormOptions
            {
                EventId = new List<Config>
                {
                    new Config
                    {
                        Value = "123456",
                        Code = "testCode",
                        Type = "pavement"
                    }
                },
                RiverOrCulvertedWaterConfig = new List<Config>
                {
                    new Config
                    {
                        Value = "riv",
                        Code = "testCode",
                        Type = "river"
                    }
                }
            };

            var mockPavementVerintOptions = new PavementVerintOptions
            {
                EventCode = 123456,
                Classification = "test",
                EventTitle = "test"
            };

            _mockVerintServiceGateway
                .Setup(_ => _.CreateVerintOnlineFormCase(It.IsAny<VerintOnlineFormRequest>()))
                .ReturnsAsync(new HttpResponse<VerintOnlineFormResponse>
                {
                    ResponseContent = new VerintOnlineFormResponse
                    {
                        VerintCaseReference = "tes ref"
                    },
                    StatusCode = HttpStatusCode.OK
                });

            _mockMailingServiceGateway
                .Setup(_ => _.Send(It.IsAny<Mail>()))
                .ReturnsAsync(new HttpResponse<string>
                {
                    ResponseContent = "test"
                });

            _floodingService = new FloodingService(
                _mockVerintServiceGateway.Object,
                _mockMailingServiceGateway.Object,
                mockPavementVerintOptions,
                mockConfirmAttributeFormOptions);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintServiceGateway()
        {
            // Act
            await _floodingService.CreateCase(_floodingRequest);

            // Assert
            _mockVerintServiceGateway.Verify(_ => _.CreateVerintOnlineFormCase(It.IsAny<VerintOnlineFormRequest>()), Times.Once);
        }

        [Fact]
        public async Task CreateCase_ShouldCallMailingServiceGateway()
        {
            // Act
            await _floodingService.CreateCase(_floodingRequest);

            // Assert
            _mockMailingServiceGateway.Verify(_ => _.Send(It.IsAny<Mail>()), Times.Once);
        }

        [Fact]
        public async Task CreateCase_ShouldReturnResponseContent()
        {
            // Act
            var result = await _floodingService.CreateCase(_floodingRequest);

            // Assert
            Assert.Contains("tes ref", result);
        }
    }
}
