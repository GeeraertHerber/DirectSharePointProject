using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class InnerError
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("request-id")]
        public Guid RequestId { get; set; }
        [JsonProperty("client-request-id")]
        public Guid ClientRequestId { get; set; }
        
        
    }
}
