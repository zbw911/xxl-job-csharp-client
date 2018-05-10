using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobClient.executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobClient.handler;
using JobClient.utils;

namespace JobClient.executor.Tests
{
    [TestClass()]
    public class XxlJobExecutorTests
    {
        [TestMethod()]
        public void registJobHandlerTest()
        {
            //var list = AssemblyManager.GetTypes(typeof(IJobHandler));
            XxlJobExecutor.TestInner();
        }
    }
}