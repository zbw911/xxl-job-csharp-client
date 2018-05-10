using System;
using System.Threading;
using JobClient.handler;

namespace JobClient.executor
{
    public class JobThread
    {
        private int jobId;
        private IJobHandler handler;

        public JobThread(int jobId, IJobHandler handler)
        {
            this.jobId = jobId;
            this.handler = handler;
        }

        internal void start()
        {
            throw new NotImplementedException();
        }

        internal void toStop(string removeOldReason)
        {
            throw new NotImplementedException();
        }
    }
}