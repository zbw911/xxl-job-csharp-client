using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.model
{
    public class HandleCallbackParam
    {


        public int logId { get; set; }
        public ReturnT<String> executeResult { get; set; }

        public HandleCallbackParam() { }
        public HandleCallbackParam(int logId, ReturnT<String> executeResult)
        {
            this.logId = logId;
            this.executeResult = executeResult;
        }

        public override string ToString()
        {
            return "HandleCallbackParam{" +
                   "logId=" + logId +
                   ", executeResult=" + executeResult +
                   '}';
        }

    }
}
