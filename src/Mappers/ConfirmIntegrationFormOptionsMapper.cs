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
            var verintConfig = request.WhatDoYouWantToReport.Equals("flood")
                ? verintOptions.Options.First(_ => _.Type.Equals(request.WhereIsTheFlood))
                : verintOptions.Options.First(_ => _.Type.Equals(request.WhatDoYouWantToReport));

            var formOptions = new ConfirmFloodingIntegrationFormOptions
            {
                EventId = verintConfig.EventCode,
                ClassCode = verintConfig.ClassCode,
                ServiceCode = verintConfig.ServiceCode,
                SubjectCode = verintConfig.SubjectCode
            };

            if (request.WhatDoYouWantToReport.Equals("flood"))
                formOptions.FloodingSourceReported =
                    attributesFormOptions.FloodingSourceReported.FirstOrDefault(_ => _.Type.Equals(request.WhereIsTheFloodingComingFrom))?.Value
                    ?? string.Empty;

            if (request.WhereIsTheFlood.Equals("home"))
            {
                formOptions.DomesticOrCommercial =
                    attributesFormOptions.CommercialOrDomestic.First(_ => _.Type.Equals("home")).Value;

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
                formOptions.DomesticOrCommercial =
                    attributesFormOptions.CommercialOrDomestic.First(_ => _.Type.Equals("business")).Value;

            if (!request.DidNotUseMap)
            {
                formOptions.XCoordinate = request.Map.Lng;
                formOptions.YCoordinate = request.Map.Lat;
            }

            return new FloodingConfiguration
            {
                ConfirmIntegrationFormOptions = formOptions,
                VerintOption = verintConfig
            };
        }
    }
}