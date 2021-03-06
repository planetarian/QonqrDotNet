﻿using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Qonqr
{
    public class QonqrApi : IDisposable
    {
        private const string _baseUrl = "http://testapi.qonqr.com/pub/zones/";
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
                throw new ArgumentNullException(nameof(apiKey));
            if (String.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException($"{nameof(apiKey)} cannot be null or whitespace.", nameof(apiKey));

            if (apiSecret == null)
                throw new ArgumentNullException(nameof(apiSecret));
            if (apiSecret.Length != _expectedApiSecretLength)
                throw new ArgumentException($"{nameof(apiSecret)} is not of the expected length.", nameof(apiSecret));

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
            if (_client == null) return;
            _client.Dispose();
            _client = null;
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
        public async Task<Zone> GetZoneAsync(uint zoneId)
        {
            string result;
            try
            {
                result = await _client.GetStringAsync($"Status/{zoneId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't get status for zone with ID {zoneId}", ex);
            }
            var zone = JsonConvert.DeserializeObject<Zone>(result);
            return zone;
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
            string uri = $"BoundingBoxStatus/{topLat}/{leftLon}/{bottomLat}/{rightLon}";
            string result;
            try
            {
                result = await _client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                string message = "Couldn't get status for zones at " +
                                 $"{topLat:0.000},{leftLon:0.000} {bottomLat:0.000},{rightLon:0.000}";
                throw new Exception(message, ex);
            }
            var zones = JsonConvert.DeserializeObject<ZoneCollection>(result);
            return zones;
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
            string uri = $"GridReferenceStatus/{utmString}/{lastChangeDate:yyyyMMddHHmmss}";
            string result;
            try
            {
                result = await _client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                string message = $"Couldn't get status for zones at {utmString} newer than {lastChangeDate}";
                throw new Exception(message, ex);
            }
            var zones = JsonConvert.DeserializeObject<ZoneCollection>(result);
            return zones;
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
            string uri = $"GridReference/{latitude}/{longitude}";
            string result;
            try
            {
                result = await _client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                string message = $"Couldn't get grid reference for location {latitude:0.000},{longitude:0.000}.";
                throw new Exception(message, ex);
            }
            var gridRef = JsonConvert.DeserializeObject<UtmGridLocation>(result);
            return gridRef;
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
                throw new ArgumentNullException(nameof(topLevel));
            if (String.IsNullOrWhiteSpace(topLevel) || !Regex.IsMatch(topLevel, "^[0-9]{1,2}[a-zA-Z]$"))
                throw new ArgumentException("Incorrect format.", nameof(topLevel));

            string result;
            try
            {
                result = await _client.GetStringAsync($"GridBreakdown/{topLevel}");
            }
            catch (Exception ex)
            {
                string message = $"Couldn't get grid breakdown for top-level reference {topLevel}.";
                throw new Exception(message, ex);
            }
            var gridRef = JsonConvert.DeserializeObject<UtmGridBreakdown>(result);
            return gridRef?.GridDetails;
        }
        
        #endregion utm grid
    }
}
