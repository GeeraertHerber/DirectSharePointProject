using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace daemon_console.Models.ReturnAnalytics
{
    public class Sentence
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("rankScore")]
        public double RankScore { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }
    }
}
