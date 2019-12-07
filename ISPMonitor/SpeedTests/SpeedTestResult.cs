using System;
using System.Collections.Generic;
using System.Text;

namespace ISPMonitor.SpeedTests
{
    public class SpeedTestResult
    {
        public string Type { get; set; }

        public DateTime Timestamp { get; set; }

        public Ping Ping { get; set; }

        public Download Download { get; set; }

        public string Isp { get; set; }

        public Interface Interface { get; set;}

        public Server Server { get; set; }

        public Result Result { get; set; }
    }
}
