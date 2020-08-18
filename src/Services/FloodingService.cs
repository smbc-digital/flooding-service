using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using StockportGovUK.NetStandard.Gateways.MailingService;
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
        private readonly IMailingServiceGateway _mailingServiceGateway;

        public FloodingService(IVerintServiceGateway verintServiceGateway, IMailingServiceGateway mailingServiceGateway)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailingServiceGateway = mailingServiceGateway;
        }

        public async Task<string> CreateCase(FloodingRequest request)
        {
            //create case object
            //create case in verint
            //generate email model
            //send email
            //return caseRef
            return "12345";
        }
    }
}