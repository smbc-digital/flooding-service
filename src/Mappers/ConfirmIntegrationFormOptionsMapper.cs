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
            var verintConfig = verintOptions.Options.First(_ => _.Type.Equals(request.WhereIsTheFlood));

            var formOptions = new ConfirmIntegrationFormOptions
            {
                FloodingSourceReported = attributesFormOptions.FloodingSourceReported.FirstOrDefault(_ => _.Type.Equals(request.WhereIsTheFloodingComingFrom))?.Value ?? string.Empty,
                EventId = verintConfig.EventCode,
                ClassCode = verintConfig.ClassCode,
                ServiceCode = verintConfig.ServiceCode,
                SubjectCode = verintConfig.SubjectCode
            };

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