using System;
using System.Linq;
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
                {
                    request.Map = await ConvertLatLng(request.Map);
                }
                else
                {
                    request.Map = new Map {
                        Lng = streetResult.Easting,
                        Lat = streetResult.Northing
                    };
                }

                var floodingLocationConfig = 
                    string.IsNullOrEmpty(request.WhereIsTheFlood) ? 
                        _verintOptions.Value.FloodingLocations.FirstOrDefault(_ => _.Type.Equals(request.WhatDoYouWantToReport)) : 
                        _verintOptions.Value.FloodingLocations.FirstOrDefault(_ => _.Type.Equals(request.WhereIsTheFlood));
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
                var result = await _gateway.GetAsync($"wfs?&service=wfs&version=1.0.0&request=getfeature&typename=flooding:vw_click_reproject&viewparams=long:{map.Lng};lat:{map.Lat};&outputformat=json&click_reproject_4326_osgb");

                var response = JsonConvert.DeserializeObject<MapResponse>(await result.Content.ReadAsStringAsync());
                var coordinates = response.features.First();

                if (coordinates == null)
                    throw new Exception("FloodingService:: ConvertLatLng:: No features found in response");

                map.Lng = coordinates.properties.EastingNorthing.Split(',')[0];
                map.Lat = coordinates.properties.EastingNorthing.Split(',')[1];

                return map;
            }
            catch (Exception ex)
            {
                throw new Exception($"FloodingService:: ConvertLatLng:: Error message: {ex.Message}", ex);
            }
        }
    }
}