using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json; 

namespace daemon_console.Models
{
    public class Application
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; } 
    
    }
}
