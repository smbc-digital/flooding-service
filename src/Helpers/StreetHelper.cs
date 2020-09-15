using System.Linq;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.Addresses;

namespace flooding_service.Helpers
{
    public interface IStreetHelper
    {
        Task<AddressSearchResult> GetStreetUniqueId(Map map);

        Task<AddressSearchResult> GetStreetDetails(Address address);
    }

    public class StreetHelper : IStreetHelper
    {
        private readonly IVerintServiceGateway _verintServiceGateway;

        public StreetHelper(IVerintServiceGateway verintServiceGateway)
        {
            _verintServiceGateway = verintServiceGateway;
        }

        public async Task<AddressSearchResult> GetStreetUniqueId(Map map)
        {
            var streetDescription = map.Street.Split(',').ToList().SkipLast(1).Aggregate("", (x, y) => x + y + ',').Trim(',');
            var streetUsrn = map.Street.Split(',').ToList().Last().Trim();
            var streetResponse = await _verintServiceGateway.GetStreetByUsrn(streetUsrn);

            if (streetResponse?.ResponseContent != null)
                return streetResponse.ResponseContent.FirstOrDefault();

            return new AddressSearchResult
            {
                USRN = streetUsrn,
                Name = streetDescription
            };
        }

        public async Task<AddressSearchResult> GetStreetDetails(Address address)
        {
            var addressResponse = await _verintServiceGateway.GetPropertyByUprn(address.PlaceRef);

            if (addressResponse?.ResponseContent != null){
                var streetResponse = await _verintServiceGateway.GetStreetByUsrn(addressResponse.ResponseContent.USRN);

                return streetResponse.ResponseContent.FirstOrDefault();
            }

            return new AddressSearchResult
            {
                Name = address.SelectedAddress,
                USRN = address.PlaceRef
            };
        }
    }
}