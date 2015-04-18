# QonqrDotNet
.NET library for the Qonqr public API.

```csharp
public async Task QonqrTestAsync()
{
    using (var api = new QonqrApi("myApplicationKey", "myApiSecret123456789123456789012"))
    {
        // Get single zone by ID
        uint zoneId = 1;
        Zone myZone = await api.GetZoneAsync(zoneId);

        Console.WriteLine("Zone {0}: {1} // {2} // {3}",
            myZone.ZoneId, myZone.CountryName, myZone.RegionName, myZone.ZoneName);
        // "Zone 1: United States // Wisconsin // Test Zone"


        // Get multiple zones within a UTM-based grid sector
        ZoneCollection utmZones = await api.GetZonesAsync("15T1H5S3Q", DateTime.Now.AddDays(-1));

        // Iterate zones in ZoneCollection
        foreach (Zone zone in utmZones)
            Console.WriteLine("{0}: {1}", zone.ZoneId, zone.ZoneName);


        // Get multiple zones within an arbitrary area
        double topLat = 36.096077;
        double leftLon = -84.127366;
        double bottomLat = 36.009494;
        double rightLon = -84.001024;
        ZoneCollection coordZones = await api.GetZonesAsync(topLat, leftLon, bottomLat, rightLon);


        // Get UTM grid reference for a coordinate location
        UtmGridLocation utmGridLoc = await api.GetUtmGridReferenceAsync(topLat, leftLon);
        Console.WriteLine(utmGridLoc.UtmGridReference); // "16S1H6S4Q"


        // Get list of UTM grid sector quadrants within a top-level UTM grid
        UtmGridArea[] utmGridQuadrants = await api.GetUtmGridBreakdownAsync("16S");
        Console.WriteLine(utmGridQuadrants.Length); // 48
        Console.WriteLine(utmGridQuadrants[0].UtmGridReference); // "16S1H1S1Q"
    }
}
```
