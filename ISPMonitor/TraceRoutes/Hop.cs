using System;
using System.Collections.Generic;
using System.Text;

namespace ISPMonitor.TraceRoutes
{
    public class Hop
    {
        public int Number { get; set; }

        public string RTT1 { get; set; }

        public string RTT2 { get; set; }

        public string RTT3 { get; set; }

        public string HostName { get; set; }

        public string HostIp { get; set; }
    }
}
