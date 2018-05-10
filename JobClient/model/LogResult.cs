using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.model
{
    public class LogResult
    {

        public int fromLineNum { get; set; }
        public int toLineNum { get; set; }
        public String logContent { get; set; }
        public bool isEnd { get; set; }
        public LogResult()
        {

        }
        public LogResult(int fromLineNum, int toLineNum, String logContent, bool isEnd)
        {
            this.fromLineNum = fromLineNum;
            this.toLineNum = toLineNum;
            this.logContent = logContent;
            this.isEnd = isEnd;
        }


    }

}
