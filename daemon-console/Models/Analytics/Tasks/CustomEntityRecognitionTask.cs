using System;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models.Analytics
{
    public class CustomEntityRecognitionTask
    {
        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }
    }
}
