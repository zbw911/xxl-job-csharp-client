using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp1.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.log.Tests
{
    [TestClass()]
    public class XxlJobLoggerTests
    {
        [TestMethod()]
        public void logTest()
        {
            for (int i = 0; i < 10; i++)
            {
                XxlJobFileAppender.contextHolder.Value = XxlJobFileAppender.makeLogFileName(System.DateTime.Now, 1);
                XxlJobLogger.log($"{i}|aaaaaaaaaaaaaaaa");
            }


        }

        [TestMethod()]
        public void logTest1()
        {
            XxlJobFileAppender.contextHolder.Value = XxlJobFileAppender.makeLogFileName(System.DateTime.Now, 1);

            var result = XxlJobFileAppender.readLog(XxlJobFileAppender.contextHolder.Value, 1);

            Console.WriteLine(result.logContent);
        }
    }
}