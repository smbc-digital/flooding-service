using System;
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
        private readonly IOptions<PavementVerintOptions> _pavementVerintOptions;
        private readonly IOptions<ConfirmAttributeFormOptions> _confirmAttributeFormOptions;
        private readonly IStreetHelper _streetHelper;
        private readonly IGateway _gateway;
        private readonly ILogger<FloodingService> _logger;

        public FloodingService(IVerintServiceGateway verintServiceGateway,
                                IMailHelper mailHelper,
                                IOptions<PavementVerintOptions> pavementVerintOptions,
                                IOptions<ConfirmAttributeFormOptions> confirmAttributeFormOptions,
                                IStreetHelper streetHelper,
                                IGateway gateway,
                                ILogger<FloodingService> logger)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailHelper = mailHelper;
            _pavementVerintOptions = pavementVerintOptions;
            _confirmAttributeFormOptions = confirmAttributeFormOptions;
            _streetHelper = streetHelper;
            _gateway = gateway;
            _logger = logger;
        }

        public async Task<string> CreateCase(FloodingRequest request)
        {
            try
            {
                var streetResult = request.DidNotUseMap ? null : await _streetHelper.GetStreetUniqueId(request.Map);

                //if (!request.DidNotUseMap)
                //{
                //    request.Map = await ConvertLatLng(request.Map);
                //}

                var crmCase = request.ToCase(_pavementVerintOptions.Value, _confirmAttributeFormOptions.Value, streetResult);
                var confirmIntegrationFormOptions = request.ToConfirmFormOptions(_confirmAttributeFormOptions.Value);
                var verintRequest = crmCase.ToConfirmIntegrationFormCase(confirmIntegrationFormOptions);

                var caseResult = await _verintServiceGateway.CreateVerintOnlineFormCase(verintRequest);

                await _mailHelper.SendEmail(request, caseResult.ResponseContent.VerintCaseReference);

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
                var client = new HttpClient();
                var result = await client.GetAsync($"http://www.bgs.ac.uk/data/webservices/CoordConvert_LL_BNG.cfc?method=LatLongToBNG&lat={map.Lat}&lon={map.Lng}");
                _logger.LogWarning($"FloodingService:: ConvertLatLng:: Response is: {JsonConvert.SerializeObject(result)}");

                var response = JsonConvert.DeserializeObject<MapResponse>(await result.Content.ReadAsStringAsync());

                map.Lat = response.Easting;
                map.Lng = response.Northing;

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