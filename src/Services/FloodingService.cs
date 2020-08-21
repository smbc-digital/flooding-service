using System;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Helpers;
using flooding_service.Mappers;
using flooding_service.Models;
using Microsoft.Extensions.Options;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;
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

        public FloodingService(IVerintServiceGateway verintServiceGateway,
                                IMailHelper mailHelper,
                                IOptions<PavementVerintOptions> pavementVerintOptions,
                                IOptions<ConfirmAttributeFormOptions> confirmAttributeFormOptions, 
                                IStreetHelper streetHelper)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailHelper = mailHelper;
            _pavementVerintOptions = pavementVerintOptions;
            _confirmAttributeFormOptions = confirmAttributeFormOptions;
            _streetHelper = streetHelper;
        }

        public async Task<string> CreateCase(FloodingRequest request)
        {
            try
            {
                var streetResult = request.DidNotUseMap ? null : await _streetHelper.GetStreetUniqueId(request.Map);
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
    }
}