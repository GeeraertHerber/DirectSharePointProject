using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using System.Text;

namespace daemon_console.Models.Analytics
{
    public class CustomMultiClassificationTask
    {
        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }
    }
}
