using ISPMonitor.SpeedTests;
using ISPMonitor.TraceRoutes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ISPMonitor
{
    public class DiagnosticTest
    {
        private List<DiagnosticResult> results { get; set; }

        private string resultsFilePath;

        private Timer testTimer;

        private object testLock = new object();

        private int speedTestServerId = -1;

        private int testCount = 0;

        public void Run(string resultsFilePath)
        {
            this.resultsFilePath = resultsFilePath;
            results = new List<DiagnosticResult>();

            SetSpeedTestServer();

            lock (testLock)
            {
                var now = DateTime.Now;
                TimeSpan startDelay = RoundUp(now, TimeSpan.FromMinutes(10)) - now;
                testTimer = new Timer(RunTest, null, startDelay, TimeSpan.FromMinutes(10));
            }

            PrintTestStatus();
        }

        DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }


        public void Stop()
        {
            lock (testLock)
            {
                testTimer.Change(Timeout.Infinite, Timeout.Infinite);
                WriteResults();
            }
        }

        private void RunTest(object state)
        {
            lock (testLock)
            {
                try
                {
                    PrintTestStatus(true);
                    SpeedTest speedTest = new SpeedTest();
                    SpeedTestResult speedTestResult = speedTest.Run(speedTestServerId);
                    TraceRoute traceRoute = new TraceRoute();
                    TraceRouteResult traceRouteResult = traceRoute.Run(speedTestResult.Server.Ip);


                    results.Add(new DiagnosticResult
                    {
                        SpeedTestResult = speedTestResult,
                        TraceRouteResult = traceRouteResult
                    });

                    WriteResults();
                    PrintTestStatus();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[{DateTime.Now:MM-dd hh:mm:ss}] Exception encountered running test: {e}");
                }
            }
        }

        private void WriteResults()
        {
            try
            {
                List<DiagnosticResult> allResults = new List<DiagnosticResult>();
                string jsonData;
                if (File.Exists(resultsFilePath))
                {
                    // Read existing json data
                    jsonData = File.ReadAllText(resultsFilePath);
                    // De-serialize to object or create new list
                    List<DiagnosticResult> existingResults = JsonConvert.DeserializeObject<List<DiagnosticResult>>(jsonData)
                                          ?? new List<DiagnosticResult>();
                    allResults.AddRange(existingResults);
                }


                // Add new results
                allResults.AddRange(results);

                // Update json data string
                jsonData = JsonConvert.SerializeObject(allResults, Formatting.Indented);
                File.WriteAllText(resultsFilePath, jsonData);
                results.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now:MM-dd hh:mm:ss}] Failed to write results to file: {e}");
            }
            
        }

        private void PrintTestStatus(bool running = false)
        {
            if (running)
            {
                Console.WriteLine($"\rTest in progres...                                                          ");
            }
            else
            {
                DateTime now = DateTime.Now;
                DateTime startTime = RoundUp(now, TimeSpan.FromMinutes(10));
                string status = testCount == 0
                    ? $"\rFirst test scheduled at {startTime:MM-dd HH:mm:ss}                                       "
                    : $"\r{testCount} tests completed.  Next test scheduled at {startTime:MM-dd HH:mm:ss}          ";
                Console.WriteLine(status);
            }
        }

        public void SetSpeedTestServer()
        {
            Console.WriteLine("Please choose a server from the list below.  Enter the id of the server you wish to use for diagnostics.");
            string servers = SpeedTest.GetServers();
            Console.WriteLine(servers);
            while(speedTestServerId == -1)
            {
                try
                {
                    string input = Console.ReadLine();
                    speedTestServerId = Convert.ToInt32(input);
                    var regex = new Regex($@"\s*{speedTestServerId.ToString()}\s*");
                    if (!regex.IsMatch(servers))
                    {
                        Console.WriteLine("That id is not present in the list. Try again...");
                        speedTestServerId = -1;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid Id.  Try again...");
                }
            }
        }
    }
}
