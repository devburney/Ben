
using NLog;
using NUnit.Framework;
using System.IO;
using Logger = NLog.Logger;

namespace TBS.PrintTest.UITest.Utils
{
    public static class FileUtils
    {
        // Quick and simple NLog implementation.
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Debug(string s)
            => logger.Debug(s);
    }
}
