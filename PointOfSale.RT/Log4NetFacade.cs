using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net
{
    public interface ILog
    {
        string Name { get; }

        void Debug(string message, Exception ex = null);
        void Error(string message, Exception ex = null);
    }

    public class Logger : ILog
    {
        public string Name { get; set; }

        public void Debug(string message, Exception ex = null)
        {
        }

        public void Error(string message, Exception ex = null)
        {
        }
    }

    public class LogManager
    {
        public static ILog GetLogger(Type type)
        {
            return new Logger { Name = type.FullName };
        }
    }
}
