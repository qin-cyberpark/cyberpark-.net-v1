using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Utilities
{
    public class Logger
    {
        private static readonly log4net.ILog _infoLogger = log4net.LogManager.GetLogger("InfoLogger");
        private static readonly log4net.ILog _errorLogger = log4net.LogManager.GetLogger("ErrorLogger");

        public static void Info(object moudule, object message)
        {
            _infoLogger.Info(string.Format("[{0}]{1}", moudule, message));
        }
        public static void Error(object moudule, object message)
        {
            _errorLogger.Error(string.Format("[{0}]{1}", moudule, message));
        }
    }
}
