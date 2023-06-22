﻿using FabricOwl;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FabricOwlController : ControllerBase
    {
        [HttpGet("{eventInstanceIds}")]
        public async Task<string> GetFabricOwl(string eventInstanceIds)
        {
            List<RCAEvents> simulEvents = await Base.GetRCA(eventInstanceIds);
            string result = string.Empty;
            foreach (var s in simulEvents)
            {
                result += JsonConvert.SerializeObject(s, Formatting.Indented) + "\n";
            }
            return result;
        }

        [HttpGet("{startTimeUTC}/{endTimeUTC}")]
        public async Task<string> GetFabricOwl(DateTime startTimeUTC, DateTime endTimeUTC)
        {

            List<RCAEvents> simulEvents = await Base.GetRCA();
            string result = string.Empty;
            foreach (var s in simulEvents)
            { 
                if (s.TimeStamp >= startTimeUTC && s.TimeStamp <= endTimeUTC)
                {
                    result += JsonConvert.SerializeObject(s, Formatting.Indented) + "\n";
                }
            }

            return result;
        }
    }
}
