using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json; 

namespace daemon_console.Models
{
    public class CreatedBy
    {
        [JsonProperty("application")]
        public Application Application { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
