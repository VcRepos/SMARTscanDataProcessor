using System;
using Topshelf;
using SMARTscan_DataProcessor.Service;
using SMARTscan_DataProcessor.Data;

namespace SMARTscan_DataProcessor
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Testing

            //VboxFileController.TransformWGS83ToOSGB(51.84380467, 0.18919483);
            //Console.ReadKey();

            var rc = HostFactory.Run(x =>
            {
                x.Service<SmscanDirector>(sc =>
                {
                    sc.ConstructUsing(hostSettings => new SmscanDirector());
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());

                });

                x.OnException(ex =>
                {

                });

                x.RunAsLocalSystem();
                x.SetDescription("Smartscan Automation: Carnell internal windows service for Smartscan data processing");
                x.SetDisplayName("Smartscan Automation");
                x.SetServiceName("Smartscan Automation");
                x.StartAutomatically();

                x.EnableServiceRecovery(r =>
                {
                    //r.RestartComputer(5, "message");
                    r.RestartService(0);
                    r.OnCrashOnly();
                });
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;

            AppLogger.LogInformation("Carnell service windows service: Topshelf installation is configured");
            new System.Threading.AutoResetEvent(false).WaitOne();
        }
        
    }
}
