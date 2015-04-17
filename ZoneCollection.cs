using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qonqr
{
    [JsonObject]
    public class ZoneCollection : IEnumerable<Zone>, IEnumerator<Zone>
    {
        public uint Count { get; set; }
        public Zone[] Zones { get; set; }


        private int _position = -1;

        [System.Runtime.CompilerServices.IndexerName("Zone")]
        public Zone this[int index]
        {
            get { return Zones[index]; }
            set { Zones[index] = value; }
        }

        public Zone Current
        {
            get { return Zones[_position]; }
        }

        public IEnumerator<Zone> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            Reset();
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

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
