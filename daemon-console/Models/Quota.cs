using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    internal class Quota
    {
        public int deleted { get; set; }
        public int remaining { get; set; }
        public string state { get; set; }
        public int total { get; set; }
        public int used { get; set; }
    }
}
