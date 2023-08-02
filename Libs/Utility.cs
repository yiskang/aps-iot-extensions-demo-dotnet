/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Developer Advocacy and Support
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using Newtonsoft.Json.Linq;
using Autodesk.Das.Models;

namespace Autodesk.Forge.Libs
{
    public static class Utility
    {
        public static JObject GetSamples(IEnumerable<Sensor> sensors, DateTime startTime, DateTime endTime, double resolution = 32)
        {
            var data = new JObject();

            foreach (Sensor sensor in sensors)
            {
                var tempValues = GenerateRandomValues(18.0, 28.0, resolution, 1.0);
                var co2Values = GenerateRandomValues(540.0, 600.0, resolution, 5.0);

                data.Add(sensor.Code, new JObject(
                    new JProperty("temp", new JArray(tempValues)),
                    new JProperty("co2", new JArray(co2Values))
                ));
            }

            var timestamps = Utility.GenerateTimestamps(startTime, endTime, resolution);

            var result = new JObject(
                new JProperty("count", new JValue(resolution)),
                new JProperty("data", data),
                new JProperty("timestamps", new JArray(timestamps))
            );

            return result;
        }

        public static List<DateTime> GenerateTimestamps(DateTime start, DateTime end, double count)
        {
            long delta = (long)Math.Floor((double)(start.ToUnixTimeMilliSeconds() - end.ToUnixTimeMilliSeconds()) / (count - 1));
            var timestamps = new List<DateTime>();
            for (var i = 0; i < count; i++)
            {
                long timestamp = start.ToUnixTimeMilliSeconds() + i * delta;
                var time = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                timestamps.Add(time.UtcDateTime);
            }
            return timestamps;
        }

        public static List<double> GenerateRandomValues(double min, double max, double count, double maxDelta)
        {
            var rnd = new Random();
            List<double> values = new List<double>();

            double lastValue = min + rnd.NextDouble() * (max - min);
            for (var i = 0; i < count; i++)
            {
                values.Add(lastValue);
                lastValue += (rnd.NextDouble() - 0.5) * 2.0 * (double)maxDelta;
                if (lastValue > max)
                {
                    lastValue = max;
                }
                if (lastValue < min)
                {
                    lastValue = min;
                }
            }
            return values;
        }
    }
}