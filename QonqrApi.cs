using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Qonqr
{
    public class QonqrApi : IDisposable
    {
        private const string _baseUrl = "http://api.qonqr.com/pub/zones/";
        private const int _expectedApiSecretLength = 32;

        private readonly string _apiKey;
        private readonly string _apiSecret;

        private HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="QonqrApi"/> class.
        /// </summary>
        /// <remarks>
        /// To obtain your api key/secret, register for developer API access at http://api.qonqr.com
        /// </remarks>
        /// <param name="apiKey">Application key from the Qonqr developer API website.</param>
        /// <param name="apiSecret">Application secret from the Qonqr developer API website.</param>
        public QonqrApi(string apiKey, string apiSecret)
        {
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");
            if (String.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("apiKey");

            if (apiSecret == null)
                throw new ArgumentNullException("apiSecret");
            if (apiSecret.Length != _expectedApiSecretLength)
                throw new ArgumentException("apiSecret");

            _apiKey = apiKey;
            _apiSecret = apiSecret;

            _client = GetClient();
        }

        /// <summary>
        /// Retrieves an instance of <see cref="HttpClient"/>
        /// prepopulated with the Qonqr API base URL and headers.
        /// </summary>
        /// <returns>HttpClient prepared for retrieving Qonqr API data.</returns>
        private HttpClient GetClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(_baseUrl) };
            client.DefaultRequestHeaders.Add("ApiKey", _apiKey);
            client.DefaultRequestHeaders.Add("ApiSecret", _apiSecret);
            return client;
        }

        /// <summary>
        /// Frees any resources required by this object.
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
        


        #region zones

        /// <summary>
        /// Asynchronously retrieves the zone with the given ID number.
        /// </summary>
        /// <param name="zoneId">Zone ID number.</param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// The returned Task's result will contain
        /// the requested Zone.
        /// </returns>
        public async Task<Zone> GetZoneAsync(int zoneId)
        {
            string result;
            try
            {
                result = await _client.GetStringAsync("Status/" + zoneId);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't get status for zone id " + zoneId, ex);
            }
            var zone = JsonConvert.DeserializeObject<Zone>(result);
            return zone;
        }

        /// <summary>
        /// Retrieves the zone with the given ID number.
        /// </summary>
        /// <param name="zoneId">Zone ID number.</param>
        /// <returns>The requested Zone.</returns>
        public Zone GetZone(int zoneId)
        {
            var task = GetZoneAsync(zoneId);
            task.Wait();
            return task.Result;
        }



        /// <summary>
        /// Asynchronously retrieves the set of up to 500 zones within the provided UTM grid reference location.
        /// </summary>
        /// <param name="topLat">Top edge longitude.</param>
        /// <param name="leftLon">Left edge longitude.</param>
        /// <param name="bottomLat">Bottom edge latitude.</param>
        /// <param name="rightLon">Right edge longitude.</param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// The returned Task's result will contain
        /// the array of up to 500 most recently-updated zones within the given area.
        /// </returns>
        public async Task<ZoneCollection> GetZonesAsync(
            double topLat, double leftLon,
            double bottomLat, double rightLon)
        {
            string uri = String.Format("BoundingBoxStatus/{0}/{1}/{2}/{3}",
                topLat, leftLon, bottomLat, rightLon);
            string result;
            try
            {
                result = await _client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                string message = String.Format(
                    "Couldn't get status for zones at {0:0.000},{1:0.000} {2:0.000},{3:0.000}",
                    topLat, leftLon, bottomLat, rightLon);
                throw new Exception(message, ex);
            }
            var zones = JsonConvert.DeserializeObject<ZoneCollection>(result);
            return zones;
        }
        
        /// <summary>
        /// Retrieves the set of up to 500 zones within the provided UTM grid reference location.
        /// </summary>
        /// <param name="topLat">Top edge longitude.</param>
        /// <param name="leftLon">Left edge longitude.</param>
        /// <param name="bottomLat">Bottom edge latitude.</param>
        /// <param name="rightLon">Right edge longitude.</param>
        /// <returns>The array of up to 500 most recently-updated zones within the given area.</returns>
        public ZoneCollection GetZones(
            double topLat, double leftLon,
            double bottomLat, double rightLon)
        {
            var task = GetZonesAsync(topLat,leftLon,bottomLat,rightLon);
            task.Wait();
            return task.Result;
        }



        /// <summary>
        /// Asynchronously retrieves the set of up to 500 zones within the provided UTM grid reference location.
        /// </summary>
        /// <param name="utmString">UTM grid reference string describing the area for which to retrieve zones.</param>
        /// <param name="lastChangeDate">Start date prior to which no zone updates will be returned.</param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// The returned Task's result will contain
        /// the array of up to 500 most recently-updated zones within the given location and time frame.
        /// </returns>
        public async Task<ZoneCollection> GetZonesAsync(string utmString, DateTime lastChangeDate)
        {
            string uri = String.Format("GridReferenceStatus/{0}/{1:yyyyMMddHHmmss}", utmString, lastChangeDate);
            string result;
            try
            {
                result = await _client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                string message = String.Format(
                    "Couldn't get status for zones at {0} newer than {1}", utmString, lastChangeDate);
                throw new Exception(message, ex);
            }
            var zones = JsonConvert.DeserializeObject<ZoneCollection>(result);
            return zones;
        }

        /// <summary>
        /// Retrieves the set of up to 500 zones within the provided UTM grid reference location.
        /// </summary>
        /// <param name="utmString">UTM grid reference string describing the area for which to retrieve zones.</param>
        /// <param name="lastChangeDate">Start date prior to which no zone updates will be returned.</param>
        /// <returns>The array of up to 500 most recently-updated zones within the given location and time frame.</returns>
        public ZoneCollection GetZones(string utmString, DateTime lastChangeDate)
        {
            var task = GetZonesAsync(utmString, lastChangeDate);
            task.Wait();
            return task.Result;
        }

        #endregion zones
        


        #region utm grid

        /// <summary>
        /// Asynchronously retrieves the UTM grid reference descriptor for the provided coordinate location.
        /// </summary>
        /// <param name="latitude">Latitude coordinate.</param>
        /// <param name="longitude">Longitude coordinate.</param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// The returned Task's result will contain
        /// the provided coordinate location's UTM grid reference descriptor.
        /// </returns>
        public async Task<UtmGridLocation> GetUtmGridReferenceAsync(double latitude, double longitude)
        {
            string uri = String.Format("GridReference/{0}/{1}", latitude, longitude);
            string result;
            try
            {
                result = await _client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                string message = String.Format(
                    "Couldn't get grid reference for location {0:0.000},{1:0.000}.",
                    latitude, longitude);
                throw new Exception(message, ex);
            }
            var gridRef = JsonConvert.DeserializeObject<UtmGridLocation>(result);
            return gridRef;
        }
        
        /// <summary>
        /// Retrieves the UTM grid reference descriptor for the provided coordinate location.
        /// </summary>
        /// <param name="latitude">Latitude coordinate.</param>
        /// <param name="longitude">Longitude coordinate.</param>
        /// <returns>The provided coordinate location's UTM grid reference descriptor.</returns>
        public UtmGridLocation GetUtmGridReference(double latitude, double longitude)
        {
            var task = GetUtmGridReferenceAsync(latitude, longitude);
            task.Wait();
            return task.Result;
        }



        /// <summary>
        /// Asynchronously retrieves a set of descriptors for all sectors within the given top-level grid.
        /// </summary>
        /// <param name="topLevel">Top-level grid name to get data for.</param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// The returned Task's result will contain
        /// the array of UtmGridArea objects within the provided top-level grid.
        /// </returns>
        public async Task<UtmGridArea[]> GetUtmGridBreakdownAsync(string topLevel)
        {
            if (topLevel == null)
                throw new ArgumentNullException("topLevel");
            if (String.IsNullOrWhiteSpace(topLevel) || !Regex.IsMatch(topLevel, "^[0-9]{1,2}[a-zA-Z]$"))
                throw new ArgumentException("Incorrect format.", "topLevel");

            string result;
            try
            {
                result = await _client.GetStringAsync("GridBreakdown/" + topLevel);
            }
            catch (Exception ex)
            {
                string message = String.Format(
                    "Couldn't get grid breakdown for top-level reference {0}.", topLevel);
                throw new Exception(message, ex);
            }
            var gridRef = JsonConvert.DeserializeObject<UtmGridBreakdown>(result);
            return gridRef == null ? null : gridRef.GridDetails;
        }

        /// <summary>
        /// Retrieves a set of descriptors for all sectors within the given top-level grid.
        /// </summary>
        /// <param name="topLevel">Top-level grid name to get data for.</param>
        /// <returns>The array of UtmGridArea objects contained within the provided top-level grid.</returns>
        public UtmGridArea[] GetUtmGridBreakdown(string topLevel)
        {
            var task = GetUtmGridBreakdownAsync(topLevel);
            task.Wait();
            return task.Result;
        }

        #endregion utm grid
    }
}
