using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace ISPMonitor.SpeedTests
{
    public class SpeedTest
    {
        private SpeedTestResult result;

        public SpeedTestResult Run(int serverId)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C speedtest.exe -v --server-id={serverId} --format=json",
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

        public static string GetServers()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C speedtest.exe -L",
                    RedirectStandardOutput = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();

            return output;
        }

        private void HandleOutput(object sender, DataReceivedEventArgs e)
        {
            var output = e.Data;
            if (sender is Process process)
            {
                if (output != null && output.Length > 0)
                {
                    result = JsonConvert.DeserializeObject<SpeedTestResult>(output);
                }
            }
        }
    }
}
