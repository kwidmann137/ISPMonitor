using System;
using System.IO;
using ISPMonitor.SpeedTests;
using ISPMonitor.TraceRoutes;
using Newtonsoft.Json;

namespace ISPMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.Write("Provide a file path to store results: ");
            string resultsFilePath = Console.ReadLine();

            if (!File.Exists(resultsFilePath))
            {
                FileStream handle = File.Create(resultsFilePath);
                handle.Close();
            }
            else
            {
                Console.WriteLine("File already exists.  Can not overwrite existing file. Press any key to exit...");
                Console.ReadLine();
                return;
            }

            DiagnosticTest test = new DiagnosticTest();
            test.Run(resultsFilePath);

            Console.WriteLine("Started.  Press any key to exit..");
            Console.ReadLine();
            Console.WriteLine("Stopping test...");
            test.Stop();
        }
    }
}
