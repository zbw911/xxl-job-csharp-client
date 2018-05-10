using ConsoleApp1.model;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.log
{
    public class XxlJobFileAppender
    {
        static ILog logger = LogManager.GetLogger("xxl - job logger");

        public static ThreadLocal<String> contextHolder = new ThreadLocal<String>();
        public static String logPath = "/data/applogs/xxl-job/jobhandler/";


        public static String makeLogFileName(DateTime triggerDate, int logId)
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            String nowFormat = triggerDate.ToString("yyyy-MM-dd");

            var filePathDateDir = Path.Combine(logPath, nowFormat);
            if (!Directory.Exists(filePathDateDir))
            {
                Directory.CreateDirectory(filePathDateDir);
            }

            // filePath/yyyy-MM-dd/9999.log
            String logFileName = filePathDateDir + ("/") + (logId) + ".log";
            return logFileName;
        }


        /**
	 * append log
	 *
	 * @param logFileName
	 * @param appendLog
	 */
        public static void appendLog(String logFileName, String appendLog)
        {

            // log
            if (appendLog == null)
            {
                appendLog = "";
            }
            appendLog += "\r\n";

            // log file
            if (logFileName == null || logFileName.Trim().Length == 0)
            {
                return;
            }


            if (!File.Exists(logFileName))
            {
                using (File.Create(logFileName)) { }
            }

            File.AppendAllText(logFileName, appendLog);

        }

        public static LogResult readLog(String logFileName, int fromLineNum)
        {

            // valid log file
            if (logFileName == null || logFileName.Trim().Length == 0)
            {
                return new LogResult(fromLineNum, 0, "readLog fail, logFile not found", true);
            }


            if (!File.Exists(logFileName))
            {
                return new LogResult(fromLineNum, 0, "readLog fail, logFile not exists", true);
            }

            // read file
            StringBuilder logContentBuffer = new StringBuilder();
            int toLineNum = 0;
            using (TextReader reader = File.OpenText(logFileName))
            {
                do
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (toLineNum++ >= fromLineNum)
                    {
                        logContentBuffer.Append(line).Append("\n");
                    }
                }
                while (true);
            }


            // result
            LogResult logResult = new LogResult(fromLineNum, toLineNum, logContentBuffer.ToString(), false);
            return logResult;

            /*
            // it will return the number of characters actually skipped
            reader.skip(Long.MAX_VALUE);
            int maxLineNum = reader.getLineNumber();
            maxLineNum++;	// 最大行号
            */
        }
    }
}
