using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qonqr
{
    [JsonObject]
    public class ZoneCollection : IEnumerable<Zone>, IEnumerator<Zone>
    {
        private int _position = -1;

        public uint Count { get; set; }
        public Zone[] Zones { get; set; }

        public Zone Current => Zones[_position];
        object IEnumerator.Current => Current;

        [System.Runtime.CompilerServices.IndexerName("Zone")]
        public Zone this[int index]
        {
            get { return Zones[index]; }
            set { Zones[index] = value; }
        }

        public IEnumerator<Zone> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool MoveNext()
        {
            _position++;
            return _position < Zones.Length;
        }

        public void Reset()
        {
            _position = 0;
        }

        public void Dispose()
        {
            Reset();
        }
    }
}
