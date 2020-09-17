using System.Linq;
using flooding_service.Controllers.Models;
using flooding_service.Models;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;

namespace flooding_service.Mappers
{
    public static class ConfirmIntegrationFormOptionsMapper
    {
        public static FloodingConfiguration ToConfig(this FloodingRequest request, ConfirmAttributeFormOptions attributesFormOptions, VerintOptions verintOptions)
        {
            var verintConfigValue = request.WhatDoYouWantToReport.Equals("flood") ? request.WhereIsTheFlood : request.WhatDoYouWantToReport;
            var verintConfig = verintOptions.Options.First(_ => _.Type.Equals(verintConfigValue));

            var formOptions = new ConfirmFloodingIntegrationFormOptions
            {
                EventId = verintConfig.EventCode,
                ClassCode = verintConfig.ClassCode,
                ServiceCode = verintConfig.ServiceCode,
                SubjectCode = verintConfig.SubjectCode
            };

            formOptions.FloodingSourceReported = attributesFormOptions.FloodingSourceReported.FirstOrDefault(_ => _.Type.Equals(request.WhereIsTheFloodingComingFrom))?.Value ?? string.Empty;
            formOptions.DomesticOrCommercial = attributesFormOptions.CommercialOrDomestic.FirstOrDefault(_ => _.Type.Equals(request.WhereIsTheFlood))?.Value ?? string.Empty;

            if (request.WhereIsTheFlood.Equals("home"))
            {
                if (request.WhereInThePropertyIsTheFlood.Equals("garage"))
                {
                    formOptions.LocationOfFlooding =
                        attributesFormOptions.FloodLocationInProperty.First(_ =>
                            _.Type.Equals(request.IsTheGarageConnectedToYourHome)).Value;
                }
                else
                {
                    formOptions.LocationOfFlooding = attributesFormOptions.FloodLocationInProperty
                        .First(_ => _.Type.Equals(request.WhereInThePropertyIsTheFlood)).Value;
                }
            }

            if (request.WhereIsTheFlood.Equals("business"))
            {
                if (request.IsTheFloodInsideOrOutsideProperty.Equals("inside"))
                {
                    formOptions.LocationOfFlooding = attributesFormOptions.FloodLocationInProperty
                                            .First(_ => _.Type.Equals(request.WhereInThePropertyIsTheFlood)).Value;
                }
                else
                {
                    formOptions.LocationOfFlooding = attributesFormOptions.FloodLocationInProperty
                                            .First(_ => _.Type.Equals("garden")).Value;
                }
            }

            formOptions.XCoordinate = request.Map?.Lng;
            formOptions.YCoordinate = request.Map?.Lat;

            return new FloodingConfiguration
            {
                ConfirmIntegrationFormOptions = formOptions,
                VerintOption = verintConfig
            };
        }
    }
}