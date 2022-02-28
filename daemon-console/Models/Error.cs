using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    internal class Error
    {
        public string code { get; set; }
        public string message { get; set; }
        public InnerError innerError { get; set; }
    }
}
