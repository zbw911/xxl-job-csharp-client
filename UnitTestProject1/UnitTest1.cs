using System;
using JobClient.log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //JsonUtil.Registry();

            Console.WriteLine(XxlJobFileAppender.makeLogFileName(System.DateTime.Now, 1));
        }
    }
}
