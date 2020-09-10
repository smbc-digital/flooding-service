using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Helpers;
using flooding_service.Mappers;
using flooding_service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;
using StockportGovUK.NetStandard.Gateways;
using StockportGovUK.NetStandard.Gateways.VerintService;

namespace flooding_service.Services
{
    public interface IFloodingService
    {
        Task<string> CreateCase(FloodingRequest request);
    }

    public class FloodingService : IFloodingService
    {
        private readonly IVerintServiceGateway _verintServiceGateway;
        private readonly IMailHelper _mailHelper;
        private readonly IOptions<VerintOptions> _verintOptions;
        private readonly IOptions<ConfirmAttributeFormOptions> _confirmAttributeFormOptions;
        private readonly IStreetHelper _streetHelper;
        private readonly IGateway _gateway;
        private readonly ILogger<FloodingService> _logger;

        public FloodingService(IVerintServiceGateway verintServiceGateway,
                                IMailHelper mailHelper,
                                IOptions<VerintOptions> verintOptions,
                                IOptions<ConfirmAttributeFormOptions> confirmAttributeFormOptions,
                                IStreetHelper streetHelper,
                                IGateway gateway,
                                ILogger<FloodingService> logger)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailHelper = mailHelper;
            _verintOptions = verintOptions;
            _confirmAttributeFormOptions = confirmAttributeFormOptions;
            _streetHelper = streetHelper;
            _gateway = gateway;
            _logger = logger;
        }

        public async Task<string> CreateCase(FloodingRequest request)
        {
            try
            {
                var streetResult = request.DidNotUseMap 
                    ? await _streetHelper.GetStreetDetails(request.Reporter.Address) 
                    : await _streetHelper.GetStreetUniqueId(request.Map);

                if (!request.DidNotUseMap)
                   request.Map = await ConvertLatLng(request.Map);
                var floodingLocationConfig = _verintOptions.Value.FloodingLocations.FirstOrDefault(_ => _.Type.Equals(request.WhereIsTheFlood));
                var configuration = request.ToConfig(_confirmAttributeFormOptions.Value, _verintOptions.Value);
                var crmCase = request.ToCase(configuration, streetResult, floodingLocationConfig);
                var verintRequest = crmCase.ToConfirmFloodingIntegrationFormCase(configuration.ConfirmIntegrationFormOptions);

                var caseResult = await _verintServiceGateway.CreateVerintOnlineFormCase(verintRequest);

                _mailHelper.SendEmail(request, caseResult.ResponseContent.VerintCaseReference);

                return caseResult.ResponseContent.VerintCaseReference;
            }
            catch (Exception ex)
            {
                throw new Exception($"FloodingService:: CreateCase, Failed to create case, exception: {ex.Message}");
            }
        }

        private async Task<Map> ConvertLatLng(Map map)
        {
            try
            {
                var result = await _gateway.GetAsync($"CoordConvert_LL_BNG.cfc?method=LatLongToBNG&lat={map.Lat}&lon={map.Lng}");
                _logger.LogWarning($"FloodingService:: ConvertLatLng:: Response is: {JsonConvert.SerializeObject(result)}");

                var response = JsonConvert.DeserializeObject<MapResponse>(await result.Content.ReadAsStringAsync());

                map.Lat = response.Northing;
                map.Lng = response.Easting;

                return map;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"FloodingService:: ConvertLatLng:: Error message: {ex.Message}");
                throw new Exception();
            }
        }
    }
}