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
                        EventCode = 111111,
                        Type = "pavement",
                        ServiceCode = "HWAY",
                        SubjectCode = "CWFD",
                        ClassCode = "SERV"
                    },
                    new Option
                    {
                        EventCode = 222222,
                        Type = "home",
                        ServiceCode = "HWAY",
                        SubjectCode = "CWFD",
                        ClassCode = "SERV"
                    },
                    new Option
                    {
                        EventCode = 333333,
                        Type = "road",
                        ServiceCode = "HWAY",
                        SubjectCode = "CWFD",
                        ClassCode = "SERV"

                    },
                    new Option
                    {
                        EventCode = 4444444,
                        Type = "business",
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
                },
                FloodLocationInProperty = new List<Config>
                {
                    new Config
                    {
                        Type = "cellarOrBasement",
                        Value = "BAS"
                    },
                    new Config
                    {
                        Type = "driveway",
                        Value = "DRV"
                    },
                    new Config
                    {
                        Type = "yes",
                        Value = "GARA"
                    },
                    new Config
                    {
                        Type = "garden",
                        Value = "GAR"
                    }
                },
                CommercialOrDomestic = new List<Config>
                {
                    new Config
                    {
                        Type = "home",
                        Value = "DOM"
                    },
                    new Config
                    {
                        Type = "business",
                        Value = "COM"
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
                WhatDoYouWantToReport = "flood",
                WhereInThePropertyIsTheFlood = "cellarOrBasement",
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

                [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenHomeJourney_AndNotInGarage()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "home",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                WhereInThePropertyIsTheFlood = "driveway"
            };

            // Act
            var result = request.ToConfig(_confirmAttributeFormOptions, _verintOptions);

            // Assert
            Assert.Equal("RIV", result.ConfirmIntegrationFormOptions.FloodingSourceReported);
            Assert.Equal("DRV", result.ConfirmIntegrationFormOptions.LocationOfFlooding);
            Assert.Equal("DOM", result.ConfirmIntegrationFormOptions.DomesticOrCommercial);
            Assert.Equal("home", result.VerintOption.Type);
            Assert.Null(result.ConfirmIntegrationFormOptions.XCoordinate);
            Assert.Null(result.ConfirmIntegrationFormOptions.YCoordinate);
        }

        [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenHomeJourney_AndInsideGarage()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "home",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                WhereInThePropertyIsTheFlood = "garage",
                IsTheGarageConnectedToYourHome = "yes"
            };

            // Act
            var result = request.ToConfig(_confirmAttributeFormOptions, _verintOptions);

            // Assert
            Assert.Equal("RIV", result.ConfirmIntegrationFormOptions.FloodingSourceReported);
            Assert.Equal("GARA", result.ConfirmIntegrationFormOptions.LocationOfFlooding);
            Assert.Equal("DOM", result.ConfirmIntegrationFormOptions.DomesticOrCommercial);
            Assert.Equal("home", result.VerintOption.Type);
            Assert.Null(result.ConfirmIntegrationFormOptions.XCoordinate);
            Assert.Null(result.ConfirmIntegrationFormOptions.YCoordinate);
        }

        [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenBusinessJourney_Outside()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "business",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                IsTheFloodInsideOrOutsideProperty = "outside",
            };

            // Act
            var result = request.ToConfig(_confirmAttributeFormOptions, _verintOptions);

            // Assert
            Assert.Equal("RIV", result.ConfirmIntegrationFormOptions.FloodingSourceReported);
            Assert.Equal("GAR", result.ConfirmIntegrationFormOptions.LocationOfFlooding);
            Assert.Equal("COM", result.ConfirmIntegrationFormOptions.DomesticOrCommercial);
            Assert.Equal("business", result.VerintOption.Type);
            Assert.Null(result.ConfirmIntegrationFormOptions.XCoordinate);
            Assert.Null(result.ConfirmIntegrationFormOptions.YCoordinate);
        }

        [Fact]
        public void ToConfirmFormOptions_ShouldMapConfigOptions_WhenBusinessJourney_Inside()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "business",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                IsTheFloodInsideOrOutsideProperty = "inside",
                WhereInThePropertyIsTheFlood = "cellarOrBasement"
            };

            // Act
            var result = request.ToConfig(_confirmAttributeFormOptions, _verintOptions);

            // Assert
            Assert.Equal("RIV", result.ConfirmIntegrationFormOptions.FloodingSourceReported);
            Assert.Equal("BAS", result.ConfirmIntegrationFormOptions.LocationOfFlooding);
            Assert.Equal("COM", result.ConfirmIntegrationFormOptions.DomesticOrCommercial);
            Assert.Equal("business", result.VerintOption.Type);
            Assert.Null(result.ConfirmIntegrationFormOptions.XCoordinate);
            Assert.Null(result.ConfirmIntegrationFormOptions.YCoordinate);
        }
    }
}
