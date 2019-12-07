using ISPMonitor.SpeedTests;
using ISPMonitor.TraceRoutes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISPMonitor

{
    public class DiagnosticResult
    {
        public SpeedTestResult SpeedTestResult { get; set; }

        public TraceRouteResult TraceRouteResult { get; set; }
    }
}
