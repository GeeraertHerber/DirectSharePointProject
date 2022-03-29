using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    public class Folder
    {
        [JsonProperty("childCount")]
        public int ChildCount { get; set; } = 0;
    }
}
