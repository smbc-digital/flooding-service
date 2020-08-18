using System.Linq;
using flooding_service.Controllers.Models;
using flooding_service.Models;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;

namespace flooding_service.Mappers
{   
    public static class ConfirmIntegrationFormOptionsMapper
    {
        public static ConfirmIntegrationFormOptions ToConfirmFormOptions(this FloodingRequest request, ConfirmAttributeFormOptions attributesFormOptions)
        {
            var formOptions = new ConfirmIntegrationFormOptions();

            formOptions.FloodingSourceReported = attributesFormOptions.RiverOrCulvertedWaterConfig.FirstOrDefault(_ => _.Type.Equals(request.WhereIsTheFloodingComingFrom)).Value ?? string.Empty;

            if(!request.DidNotUseMap)
            {
                formOptions.XCoordinate = request.Map.Lng;
                formOptions.YCoordinate = request.Map.Lat;
            }

            return formOptions;
        }
    }
}