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

        public ConfirmIntegrationFormOptionsMapperTests()
        {
            _confirmAttributeFormOptions = new ConfirmAttributeFormOptions
            {
                RiverOrCulvertedWaterConfig = new List<Config>
                {
                    new Config
                    {
                        Type = "river",
                        Code = "CONF_ATTRIBUTE_FSRC_CODE",
                        Value = "RIV"
                    },
                    new Config
                    {
                        Type = "culverted",
                        Code = "CONF_ATTRIBUTE_FSRC_CODE",
                        Value = "CULV"
                    }
                }
            };
        }

        [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenMapUsed()
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
            var result = request.ToConfirmFormOptions(_confirmAttributeFormOptions);

            // Assert
            Assert.Equal("RIV", result.FloodingSourceReported);
            Assert.Equal("lat", result.YCoordinate);
            Assert.Equal("lng", result.XCoordinate);
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
            var result = request.ToConfirmFormOptions(_confirmAttributeFormOptions);

            // Assert
            Assert.Equal("RIV", result.FloodingSourceReported);
            Assert.Null(result.YCoordinate);
            Assert.Null(result.XCoordinate);
        }
    }
}
