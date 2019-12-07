using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ISPMonitor.TraceRoutes
{
    public class TraceRoute
    {
        TraceRouteResult result;

        Regex HopRegex = new Regex(@"\s*(\d+)\s+(.\d+ ms)\s+(.\d+ ms)\s+(.\d+ ms)\s+(.*)", RegexOptions.Compiled);

        Regex HostNameAndIpRegex = new Regex(@"\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\]", RegexOptions.Compiled);

        public TraceRouteResult Run(string target)
        {
            result = new TraceRouteResult();

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C tracert {target}",
                    RedirectStandardOutput = true
                }
            };

            process.OutputDataReceived += HandleOutput;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();

            return result;
        }

        private void HandleOutput(object sender, DataReceivedEventArgs e)
        {
            var output = e.Data;
            if (sender is Process process)
            {
                if (output != null && output.Length > 0)
                {
                    Match hopMatch = HopRegex.Match(output);
                    if (hopMatch != null && hopMatch.Groups.Count == 6)
                    {
                        string hostName = null;
                        string hostIp = null;

                        Match hostNameMatch = HostNameAndIpRegex.Match(hopMatch.Groups[5].Value);
                        if (hostNameMatch != null && hostNameMatch.Success)
                        {
                            hostIp = hostNameMatch.Value.Replace("[", string.Empty).Replace("]", string.Empty);
                            hostName = hopMatch.Groups[5].Value.Replace($" {hostNameMatch.Value}", string.Empty);
                        }
                        else
                        {
                            hostIp = hopMatch.Groups[5].Value;
                        }

                        result.Hops.Add(new Hop
                        {
                            Number = Convert.ToInt32(hopMatch.Groups[1].Value),
                            RTT1 = hopMatch.Groups[2].Value,
                            RTT2 = hopMatch.Groups[3].Value,
                            RTT3 = hopMatch.Groups[4].Value,
                            HostName = hostName,
                            HostIp = hostIp
                        });
                    }
                }
            }
        }
    }
}
