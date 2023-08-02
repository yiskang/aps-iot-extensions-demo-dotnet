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

using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Autodesk.Das.Models;
using Autodesk.Forge.Libs;

namespace Autodesk.Das.Controllers
{
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly IDataStore dataStore;

        public IotController(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        [HttpGet]
        [Route("iot/samples")]
        public async Task<IActionResult> GetSamplesAsync([FromQuery] string? start, [FromQuery] string? end, [FromQuery] double? resolution)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end) || !resolution.HasValue)
                    throw new InvalidDataException("Missing some of the required parameters: \"start\", \"end\", \"resolution\".");

                var sensors = this.dataStore.GetCollection<Sensor>();

                var startTime = DateTime.Parse(start, null, System.Globalization.DateTimeStyles.RoundtripKind);
                var endTime = DateTime.Parse(end, null, System.Globalization.DateTimeStyles.RoundtripKind);

                double resol = resolution.HasValue ? resolution.Value : 0;
                var result = Utility.GetSamples(sensors.AsQueryable(), startTime, endTime, resol);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("iot/channels")]
        public async Task<IActionResult> GetChannelsAsync()
        {
            try
            {
                var data = this.dataStore.GetCollection<Channel>();
                var rawChannels = data.AsQueryable();

                var channels = new Dictionary<string, Channel>();
                foreach(var channel in rawChannels)
                {
                    channels.Add(channel.Code, channel);
                }

                return Ok(channels);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("iot/sensors")]
        public async Task<IActionResult> GetSensorsAsync([FromQuery] string? code)
        {
            try
            {
                var data = this.dataStore.GetCollection<Sensor>();
                var rawSensors = data.AsQueryable();

                var sensors = new Dictionary<string, Sensor>();
                foreach(var sensor in rawSensors)
                {
                    sensors.Add(sensor.Code, sensor);
                }

                if (!string.IsNullOrWhiteSpace(code))
                    return Ok(sensors[code]);

                return Ok(sensors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}