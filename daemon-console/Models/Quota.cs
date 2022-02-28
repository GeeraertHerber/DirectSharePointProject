using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    internal class Quota
    {
        public int deleted { get; set; }
        public long remaining { get; set; }
        public string state { get; set; }
        public long total { get; set; }
        public int used { get; set; }
    }
}
