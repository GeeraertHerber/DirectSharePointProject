using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    public class SiteFile
    {
        public string mimeType { get; set; }
        public Hashes hashes { get; set; }
    }
}
