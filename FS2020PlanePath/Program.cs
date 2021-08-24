using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS2020PlanePath
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static public string sAppVersion = "1.4.1";
        static public string sInvalidWaypointName = "Unknown Waypoint";
        static public bool bLogErrorsWritten = false;

        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainPage());
        }

        public static string ErrorLogFile()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PilotPathRecorderLog.txt";
        }

        public static void ErrorLogging(string sAdtlMsg, Exception ex)
        {
            string strPath = ErrorLogFile(); 
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Version: " + Program.sAppVersion);
                sw.WriteLine("Additional Information: " + sAdtlMsg);
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End=============");
            }
            bLogErrorsWritten = true;
        }
    }
}
