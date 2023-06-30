using flooding_service.Controllers.Models;
using flooding_service.Models;
using StockportGovUK.NetStandard.Gateways.Models.Addresses;
using StockportGovUK.NetStandard.Gateways.VerintService;

namespace flooding_service.Helpers
{
    public interface IStreetHelper
    {
        Task<FloodingAddress> GetStreetUniqueId(Map map);

        Task<FloodingAddress> GetStreetDetails(Address address);
    }

    public class StreetHelper : IStreetHelper
    {
        private readonly IVerintServiceGateway _verintServiceGateway;

        public StreetHelper(IVerintServiceGateway verintServiceGateway)
        {
            _verintServiceGateway = verintServiceGateway;
        }

        public async Task<FloodingAddress> GetStreetUniqueId(Map map)
        {
            var streetDescription = map.Street.Split(',').ToList().SkipLast(1).Aggregate("", (x, y) => x + y + ',').Trim(',');
            var streetUsrn = map.Street.Split(',').ToList().Last().Trim();
            var streetResponse = await _verintServiceGateway.GetStreetByUsrn(streetUsrn);

            if (streetResponse?.ResponseContent != null)
            {
                var response = streetResponse.ResponseContent.FirstOrDefault();

                return new FloodingAddress
                {
                    Name = response.Name,
                    USRN = response.USRN,
                    UniqueId = response.UniqueId
                };
            }

            return new FloodingAddress
            {
                USRN = streetUsrn,
                Name = streetDescription
            };
        }

        public async Task<FloodingAddress> GetStreetDetails(Address address)
        {
            var addressResponse = await _verintServiceGateway.GetPropertyByUprn(address.PlaceRef);

            if (addressResponse?.ResponseContent != null){
                var streetResponse = await _verintServiceGateway.GetStreetByUsrn(addressResponse.ResponseContent.USRN);
                var response = streetResponse.ResponseContent.FirstOrDefault();

                return new FloodingAddress
                {
                    Name = response.Name,
                    USRN = response.USRN,
                    UniqueId = response.UniqueId,
                    Easting = addressResponse.ResponseContent.Easting,
                    Northing = addressResponse.ResponseContent.Northing
                };
            }

            return new FloodingAddress
            {
                Name = address.SelectedAddress,
                USRN = address.PlaceRef
            };
        }
    }
}