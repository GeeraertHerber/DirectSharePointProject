using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    public class Group
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
