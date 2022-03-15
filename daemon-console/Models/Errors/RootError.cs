using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class RootError
    {
        [JsonProperty("error")]
        public Error Error { get; set; }
    }
}
