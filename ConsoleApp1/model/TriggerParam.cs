using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.model
{
    public class TriggerParam
    {
        public int jobId { get; set; }
        public String executorHandler { get; set; }
        public String executorParams { get; set; }
        public String executorBlockStrategy { get; set; }
        public int logId { get; set; }
        public long logDateTim { get; set; }
        public String glueType { get; set; }
        public String glueSource { get; set; }
        public long glueUpdatetime { get; set; }
        public int broadcastIndex { get; set; }
        public int broadcastTotal { get; set; }

        public override string ToString()
        {
            return "TriggerParam{" +
                "jobId=" + jobId +
                ", executorHandler='" + executorHandler + '\'' +
                ", executorParams='" + executorParams + '\'' +
                ", executorBlockStrategy='" + executorBlockStrategy + '\'' +
                ", logId=" + logId +
                ", logDateTim=" + logDateTim +
                ", glueType='" + glueType + '\'' +
                ", glueSource='" + glueSource + '\'' +
                ", glueUpdatetime=" + glueUpdatetime +
                ", broadcastIndex=" + broadcastIndex +
                ", broadcastTotal=" + broadcastTotal +
                '}';
        }
    }
}
