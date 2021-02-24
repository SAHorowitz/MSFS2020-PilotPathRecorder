using System;
using System.Linq;
using System.Collections.Generic;

namespace FS2020PlanePath
{

    public class ReplayFlightDataGenerator : AbstractFlightDataGenerator
    {

        internal ReplayFlightDataGenerator(
            FS2020_SQLLiteDB dbAccessor, 
            (int flightNo, string flightName, int segmentDurationSecs) context
        )
        {
            segmentDurationSecs = context.segmentDurationSecs;
            flightPath = dbAccessor.GetFlightPathSinceTimestamp(context.flightNo, 0);
            generatorName = $"Replay of {context.flightName}";
            Console.WriteLine($"{generatorName}; size({flightPath.Count})");
        }

        public override string Name => generatorName;

        internal override List<FlightPathData> GetFlightPathSince(long startingTimestamp)
        {
            if (flightPath.Count == 0)
            {
                return new List<FlightPathData>();
            }
            long initialTimestamp = Math.Max(nextStartingTimestamp, Math.Max(startingTimestamp, flightPath[0].timestamp));
            nextStartingTimestamp = initialTimestamp + segmentDurationSecs * TICKS_PER_SECOND;
            return flightPath.FindAll(fp => fp.timestamp >= initialTimestamp && fp.timestamp < nextStartingTimestamp).ToList();
        }

        private const int TICKS_PER_SECOND = 10000000;

        private string generatorName;
        private int segmentDurationSecs;
        private List<FlightPathData> flightPath;
        private long nextStartingTimestamp;

    }

}
