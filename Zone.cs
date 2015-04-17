using System;

namespace Qonqr
{
    public class Zone
    {
        public uint ZoneId { get; set; }
        public string ZoneName { get; set; }
        public uint RegionId { get; set; }
        public string RegionName { get; set; }
        public uint CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ZoneControlState ControlState { get; set; }
        public DateTime DateCapturedUtc { get; set; }
        public DateTime LeaderSinceDateUtc { get; set; }
        public uint LegionCount { get; set; }
        public uint SwarmCount { get; set; }
        public uint FacelessCount { get; set; }
        public DateTime LastUpdateDateUtc { get; set; }
    }

    public enum ZoneControlState
    {
        Uncontrolled = 0,
        Legion,
        Swarm,
        Faceless
    }
}
