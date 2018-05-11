using JobClient.handler;
using JobClient.handler.attribute;
using JobClient.log;
using JobClient.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
