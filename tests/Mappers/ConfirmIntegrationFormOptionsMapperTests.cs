using System.Collections.Generic;
using flooding_service.Controllers.Models;
using flooding_service.Mappers;
using flooding_service.Models;
using Xunit;

namespace flooding_service_tests.Mappers
{
    public class ConfirmIntegrationFormOptionsMapperTests
    {
        private readonly ConfirmAttributeFormOptions _confirmAttributeFormOptions;
        private readonly VerintOptions _verintOptions;

        public ConfirmIntegrationFormOptionsMapperTests()
        {
            _verintOptions = new VerintOptions
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
                    },
                    new Option
                    {
                        EventCode = 1111111,
                        Type = "road",
                        ServiceCode = "HWAY",
                        SubjectCode = "CWFD",
                        ClassCode = "SERV"

                    }
                }
            };

            _confirmAttributeFormOptions = new ConfirmAttributeFormOptions
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
            };
        }

        [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenMapUsed_PavementJourney()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "pavement",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                Map = new Map
                {
                    Lat = "lat",
                    Lng = "lng",
                    Street = "street"
                }
            };

            // Act
            var result = request.ToConfig(_confirmAttributeFormOptions, _verintOptions);

            // Assert
            Assert.Equal("RIV", result.ConfirmIntegrationFormOptions.FloodingSourceReported);
            Assert.Equal("lat", result.ConfirmIntegrationFormOptions.YCoordinate);
            Assert.Equal("lng", result.ConfirmIntegrationFormOptions.XCoordinate);
        }

        [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenMapNotUsed()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "home",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood"
            };

            // Act
            var result = request.ToConfig(_confirmAttributeFormOptions, _verintOptions);

            // Assert
            Assert.Equal("RIV", result.ConfirmIntegrationFormOptions.FloodingSourceReported);
            Assert.Equal("home", result.VerintOption.Type);
            Assert.Null(result.ConfirmIntegrationFormOptions.XCoordinate);
            Assert.Null(result.ConfirmIntegrationFormOptions.YCoordinate);
        }

        [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenMapUsed_RoadJourney()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "road",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                Map = new Map
                {
                    Lat = "lat",
                    Lng = "lng",
                    Street = "street"
                }
            };

            // Act
            var result = request.ToConfig(_confirmAttributeFormOptions, _verintOptions);

            // Assert
            Assert.Equal("RIV", result.ConfirmIntegrationFormOptions.FloodingSourceReported);
            Assert.Equal("road", result.VerintOption.Type);
            Assert.Equal("lat", result.ConfirmIntegrationFormOptions.YCoordinate);
            Assert.Equal("lng", result.ConfirmIntegrationFormOptions.XCoordinate);
        }
    }
}
