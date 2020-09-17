using System.Collections.Generic;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Models.ContactDetails;

namespace flooding_service.Controllers.Models
{
    public class FloodingRequest
    {
        public string WhatDoYouWantToReport { get; set; }
        public string WhereIsTheFloodingComingFrom { get; set; }
        public string WhereIsTheFlood { get; set; }
        public string IsTheFloodingBlockingTheWholePavementOrCausing { get; set; }
        public string IsTheFloodingBlockingTheWholeRoadOrCausing { get; set; }
        public string IsTheFloodInsideOrOutsideProperty { get; set; }
        public string WhereInThePropertyIsTheFlood { get; set; }
        public string TellUsWhereTheFloodIsComingFrom { get; set; }
        public string IsTheGarageConnectedToYourHome { get; set; }
        public Map Map { get; set; }
        public string TellUsABoutTheFlood { get; set; }
        public ContactDetails Reporter { get; set; }
        public string HowWouldYouLikeToBeContacted { get; set; }
        public bool DidNotUseMap => WhereIsTheFlood.Equals("home") || WhereIsTheFlood.Equals("business");
    }

    public class Map
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Street { get; set; }
    }

    public class Properties    {
        [JsonProperty("click_reproject_4326_osgb")]
        public string EastingNorthing { get; set; } 
    }

    public class Feature    {
        public Properties properties { get; set; } 
    }

    public class MapResponse    {
        public List<Feature> features { get; set; } 
    }
}