﻿using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;

namespace flooding_service.Models
{
    public class FloodingConfiguration
    {
        public Option VerintOption { get; set; }

        public ConfirmFloodingIntegrationFormOptions ConfirmIntegrationFormOptions { get; set; }
    }
}
