using System;
using System.Collections.Generic;
using System.Text;

namespace ISPMonitor.SpeedTests
{
    public class Interface
    {
        public string InternalIp { get; set; }

        public string Name { get; set; }

        public string MacAddr { get; set; }

        public bool IsVpn { get; set; }

        public string ExternalIp { get; set; }
    }
}
