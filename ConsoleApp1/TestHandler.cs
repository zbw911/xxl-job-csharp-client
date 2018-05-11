using JobClient.handler;
using JobClient.handler.attribute;
using JobClient.log;
using JobClient.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestProject1
{

    [JobHander(Name = "mytest")]
    class TestHandler2 : IJobHandler
    {
        public override ReturnT<string> execute(params string[] param)
        {
            XxlJobLogger.log("test");
            return ReturnT<string>.SUCCESS;
        }
    }



    [JobHander(Name = "logTimeJob")]
    class TestHandler33 : IJobHandler
    {
        public override ReturnT<string> execute(params string[] param)
        {
            for (int i = 0; i < 1000; i++)
            {
                XxlJobLogger.log("test" + i);
                Thread.Sleep(1 * 1000);
            }

            return ReturnT<string>.SUCCESS;
        }
    }
}
