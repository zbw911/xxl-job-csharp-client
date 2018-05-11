using JobClient.utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobClient.log
{
    public class XxlJobLogger
    {
        static ILog logger = Log4netManager.GetLogger("xxl - job logger");
        public static void log(string appendLog)
        {

            // logFileName
            string logFileName = XxlJobFileAppender.contextHolder.Value;
            if (logFileName == null || logFileName.Trim().Length == 0)
            {
                return;
            }

            // "yyyy-MM-dd HH:mm:ss [ClassName]-[MethodName]-[LineNumber]-[ThreadName] log";
            //StackTraceElement[] stackTraceElements = new Throwable().getStackTrace();
            //StackTraceElement callInfo = stackTraceElements[1];

            StackTrace st = new StackTrace(true);
            var frame = st.GetFrame(0);

            StringBuilder stringBuffer = new StringBuilder();
            stringBuffer.Append(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Append(" ")
                .Append("[" + frame.GetFileName() + "]").Append("-")
                .Append("[" + frame.GetMethod().Name + "]").Append("-")
                .Append("[" + frame.GetFileLineNumber() + "]").Append("-")
                .Append("[" + Thread.CurrentThread.ManagedThreadId + "]").Append(" ")
                .Append(appendLog != null ? appendLog : "");
            string formatAppendLog = stringBuffer.ToString();

            // appendlog
            XxlJobFileAppender.appendLog(logFileName, formatAppendLog);

            //logger.warn("[{}]: {}", logFileName, formatAppendLog);
        }
        public static void log(String appendLogPattern, params object[] appendLogArguments)
        {
            String appendLog = string.Format(appendLogPattern, appendLogArguments);
            log(appendLog);
        }

    }
}
