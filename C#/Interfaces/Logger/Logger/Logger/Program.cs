using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Logger
{
    using System;
    using System.ComponentModel;
    using static global::Logger.Logger.Program;

    namespace Logger
    {
        class Program
        {
            public interface ILogger
            {
                void Log(string message);
            }

            public class FileLogger : ILogger
            {
                public void Log(string message)
                {
                    string directoryPath = $@"C:\logs";
                    string filePath = Path.Combine(directoryPath,"log.txt");
                    if (!Directory.Exists(filePath)) { Directory.CreateDirectory(directoryPath); }
                    File.AppendAllText(filePath, message+ "\n");
                }
            }

            public class DateBaseLogger : ILogger
            {
                public void Log(string message) { Console.WriteLine($"Logging to database{message}"); }
            }

            public class Application
            {
                private readonly ILogger _logger;

                public Application(ILogger logger)
                {
                    _logger = logger;
                }
                public void DoWork()
                {
                    _logger.Log("Work Started");
                    _logger.Log("Work DONE!");
                }

            }

            static void Main(string[] args)
            {
                ILogger fileLogger = new FileLogger();
                Application app = new Application(fileLogger);
                app.DoWork();

                ILogger dbLogger = new DateBaseLogger();
                app = new Application(dbLogger);
                app.DoWork();
            }
        }
    }
}
