using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json; 

namespace daemon_console.Models
{
    public class User
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
