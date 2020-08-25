using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Helpers;
using flooding_service.Models;
using flooding_service.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Gateways;
using StockportGovUK.NetStandard.Gateways.Response;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.ContactDetails;
using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using Xunit;

namespace flooding_service_tests.Services
{
    public class FloodingServiceTests
    {
        private readonly FloodingService _floodingService;
        private readonly Mock<IVerintServiceGateway> _mockVerintServiceGateway = new Mock<IVerintServiceGateway>();
        private readonly Mock<IMailHelper> _mockMailHelper = new Mock<IMailHelper>();
        private readonly Mock<IStreetHelper> _mockStreetHelper = new Mock<IStreetHelper>();
        private readonly Mock<IGateway> _mockGateway = new Mock<IGateway>();
        private readonly Mock<ILogger<FloodingService>> _mockLogger = new Mock<ILogger<FloodingService>>();
        private FloodingRequest _floodingRequest = new FloodingRequest
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
            var mockConfirmAttributeFromOptions = new Mock<IOptions<ConfirmAttributeFormOptions>>();
            mockConfirmAttributeFromOptions
                .SetupGet(_ => _.Value)
                .Returns(new ConfirmAttributeFormOptions
                {
                    FloodingSourceReported = new List<Config>
                    {
                        new Config
                        {
                            Type = "river",
                            Value = "RIV"
                        },
                        new Config
                        {
                            Type = "culverted",
                            Value = "CULV"
                        }
                    }
                });

            var mockPavementVerintOptions = new Mock<IOptions<VerintOptions>>();
            mockPavementVerintOptions
                .SetupGet(_ => _.Value)
                .Returns(new VerintOptions
                {
                    Options = new List<Option>
                {
                    new Option
                    {
                        EventCode = 2002592,
                        Type = "pavement",
                        ServiceCode = "HWAY",
                        SubjectCode = "CWFD",
                        ClassCode = "SERV"
                    },
                    new Option
                    {
                        EventCode = 0013254,
                        Type = "home",
                        ServiceCode = "HWAY",
                        SubjectCode = "CWFD",
                        ClassCode = "SERV"

                    }
                }
                });

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

            _mockStreetHelper
                .Setup(_ => _.GetStreetUniqueId(It.IsAny<Map>()))
                .ReturnsAsync(new AddressSearchResult
                {
                    UniqueId = "123456",
                    USRN = "654321",
                    Name = "TestName"
                });

            var mapResponse = new MapResponse
            {
                Easting = "56789",
                Northing = "123456"
            };

            _mockGateway
                .Setup(_ => _.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(mapResponse))
                });

            _floodingService = new FloodingService(
                _mockVerintServiceGateway.Object,
                _mockMailHelper.Object,
                mockPavementVerintOptions.Object,
                mockConfirmAttributeFromOptions.Object,
                _mockStreetHelper.Object,
                _mockGateway.Object,
                _mockLogger.Object);
        }

        [Fact(Skip="use httpClient for debugging")]
        public async Task CreateCase_ShouldCallStreetHelper_IfMapUsed()
        {
            // Act
            await _floodingService.CreateCase(_floodingRequest);

            // Assert
            _mockStreetHelper.Verify(_ => _.GetStreetUniqueId(It.IsAny<Map>()), Times.Once);
        }

        [Fact(Skip = "use httpClient for debugging")]
        public async Task CreateCase_ShouldNotCallStreetHelper_IfMapNotUsed()
        {
            // Arrange
            var floodingRequest = new FloodingRequest
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
                WhereIsTheFlood = "home",
                WhereIsTheFloodingComingFrom = "river"
            };

            // Act
            await _floodingService.CreateCase(floodingRequest);

            // Assert
            _mockStreetHelper.Verify(_ => _.GetStreetUniqueId(It.IsAny<Map>()), Times.Never);
        }

        [Fact(Skip = "use httpClient for debugging")]
        public async Task CreateCase_ShouldCallVerintServiceGateway()
        {
            // Act
            await _floodingService.CreateCase(_floodingRequest);

            // Assert
            _mockVerintServiceGateway.Verify(_ => _.CreateVerintOnlineFormCase(It.IsAny<VerintOnlineFormRequest>()), Times.Once);
        }

        [Fact(Skip = "use httpClient for debugging")]
        public async Task CreateCase_ShouldCallMailingServiceGateway()
        {
            // Act
            await _floodingService.CreateCase(_floodingRequest);

            // Assert
            _mockMailHelper.Verify(_ => _.SendEmail(It.IsAny<FloodingRequest>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(Skip = "use httpClient for debugging")]
        public async Task CreateCase_ShouldReturnResponseContent()
        {
            // Act
            var result = await _floodingService.CreateCase(_floodingRequest);

            // Assert
            Assert.Contains("tes ref", result);
        }

        [Fact(Skip = "use httpClient for debugging")]
        public async Task CreateCase_ShouldCallGatewayGetAsync_ConvertLatLngIfMapUsed()
        {
            // Act
            await _floodingService.CreateCase(_floodingRequest);

            // Assert
            _mockGateway.Verify(_ => _.GetAsync(It.IsAny<string>()), Times.Once);
            Assert.NotEqual("50.23", _floodingRequest.Map.Lat);
            Assert.NotEqual("-2.255", _floodingRequest.Map.Lng);
            Assert.Equal("56789", _floodingRequest.Map.Lat);
            Assert.Equal("123456", _floodingRequest.Map.Lng);
        }
    }
}
