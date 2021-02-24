using System;
using System.Collections.Generic;

namespace FS2020PlanePath
{

    public abstract class AbstractFlightDataGenerator : IFlightDataGenerator
    {

        public abstract string Name { get; }

        public IEnumerator<FlightPathData> NextFlightPathSegment()
        {
            long initialTimestamp = startingTimestamp;
            List<FlightPathData> nextFlightPathSegment = GetFlightPathSince(initialTimestamp);
            int segmentSize = nextFlightPathSegment.Count;
            if (segmentSize > 0)
            {
                startingTimestamp = nextFlightPathSegment[segmentSize - 1].timestamp + 1;
            }
            //Console.WriteLine($"delivering({segmentSize}) paths from({initialTimestamp}) to({startingTimestamp})");
            return nextFlightPathSegment.GetEnumerator();
        }

        internal abstract List<FlightPathData> GetFlightPathSince(long timestamp);

        private long startingTimestamp;

    }

}
