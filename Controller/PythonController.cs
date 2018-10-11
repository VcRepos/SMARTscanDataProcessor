using System;
using System.Diagnostics;
using System.IO;
using SMARTscan_DataProcessor.Data;

namespace SMARTscan_DataProcessor.Controller
{
    public class PythonController
    {
        public ProcessStartInfo myProcessStartInfo;
        public string PyfilePath { get; set; }
        public string PyEnvironment { get; set; }

        public PythonController()
        {
            myProcessStartInfo = new ProcessStartInfo()
            {
                FileName = Global.Pyenvironment,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
        }

        /// <summary>
        /// Start to run python script using specific python environment
        /// </summary>
        /// <param name="pyfilePath"></param>
        /// <param name="statement1"></param>
        public void Start(string pyfilePath, string statement1)
        {
            PyfilePath = pyfilePath;
            Process myProcess = new Process();
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.Arguments = string.Format($"{pyfilePath} {statement1}");
            myProcess.StartInfo = myProcessStartInfo;
            myProcess.Start();
            using (StreamReader myStreamReader = myProcess.StandardOutput)
            {
                string myString = myStreamReader.ReadLine();
                AppLogger.LogInformation(myString);
                myProcess.WaitForExit();
                myProcess.Close();
            }
        }

    }
}
