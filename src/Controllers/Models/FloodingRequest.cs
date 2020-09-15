using System;
using System.Collections.Generic;
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
        public string click_reproject_4326_osgb { get; set; } 
    }

    public class Feature    {
        public string type { get; set; } 
        public string id { get; set; } 
        public object geometry { get; set; } 
        public Properties properties { get; set; } 
    }

    public class MapResponse    {
        public string type { get; set; } 
        public List<Feature> features { get; set; } 
        public int totalFeatures { get; set; } 
        public int numberMatched { get; set; } 
        public int numberReturned { get; set; } 
        public DateTime timeStamp { get; set; } 
        public object crs { get; set; } 
    }
}