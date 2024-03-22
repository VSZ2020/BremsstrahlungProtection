using System.Diagnostics;

namespace BSP.BL.Infrastructure
{
    public class Logger
    {
        static Logger()
        {
            logFilePath = Path.Combine(logFilename);
        }

        static string logFilePath = "";
        const string logFilename = "log.txt";
        const string messageFormat = "{0:dd.MM.yyyy HH:mm}\t[{1}]: {2}";

        public static void Log(string message, string title = "BSP")
        {
            LogToConsole(message, title);
        }

        public static void LogToFile(string message, string title = "BSP")
        {
            using (StreamWriter wr = new StreamWriter(logFilePath, true, System.Text.Encoding.UTF8))
            {
                wr.WriteLine(string.Format(messageFormat, DateTime.Now, title, message));
            }
            
        }

        public static void LogToConsole(string message, string title = "BSP")
        {
            Trace.WriteLine(string.Format(messageFormat, DateTime.Now, title, message));
            //Trace.Flush();
        }
    }
}
