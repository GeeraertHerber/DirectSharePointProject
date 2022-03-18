using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace daemon_console.Models.Analytics
{
    public class EntityRecognitionPiiTask
    {
        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }
    }
}
