using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    public class Owner
    {
        [JsonProperty("group")]
        public Group Group { get; set; }
    }
}
