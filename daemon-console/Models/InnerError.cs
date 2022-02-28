using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class InnerError
    {
        [JsonProperty("code")]
        public string code { get; set; }
        public DateTime date { get; set; }
        [JsonProperty("request-id")]
        public Guid requestId { get; set; }
        [JsonProperty("client-request-id")]
        public Guid clientRequestId { get; set; }
        
        
    }
}
