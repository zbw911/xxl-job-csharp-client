using JobClient.executor;
using JobClient.glue;
using JobClient.handler;
using JobClient.log;
using JobClient.model;
using JobClient.utils;
using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.impl
{
    public class ExecutorBizImpl
    {
        private static ILog logger = Log4netManager.GetLogger(typeof(ExecutorBizImpl));

        public ReturnT<string> beat()
        {
            return ReturnT<string>.SUCCESS;
        }


        public ReturnT<String> idleBeat(int jobId)
        {

            // isRunningOrHasQueue
            bool isRunningOrHasQueue = false;
            JobThread jobThread = XxlJobExecutor.loadJobThread(jobId);
            if (jobThread != null && jobThread.isRunningOrHasQueue())
            {
                isRunningOrHasQueue = true;
            }

            if (isRunningOrHasQueue)
            {
                return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "job thread is running or has trigger queue.");
            }
            return ReturnT<string>.SUCCESS;


        }


        public ReturnT<String> kill(int jobId)
        {
            // kill handlerThread, and create new one
            JobThread jobThread = XxlJobExecutor.loadJobThread(jobId);
            if (jobThread != null)
            {
                XxlJobExecutor.removeJobThread(jobId, "人工手动终止");
                return ReturnT<string>.SUCCESS;
            }

            return new ReturnT<String>(ReturnT<string>.SUCCESS_CODE, "job thread aleady killed.");
        }


        public ReturnT<LogResult> log(long logDateTim, int logId, int fromLineNum)
        {
            // log filename: yyyy-MM-dd/9999.log
            //todo:这里时间戳转换应该会问题
            String logFileName = XxlJobFileAppender.makeLogFileName(TimeUtil.ToTime(logDateTim), logId);

            LogResult logResult = XxlJobFileAppender.readLog(logFileName, fromLineNum);
            return new ReturnT<LogResult>(logResult);
        }

        public ReturnT<String> run(TriggerParam triggerParam)
        {

            //// load old：jobHandler + jobThread
            JobThread jobThread = XxlJobExecutor.loadJobThread(triggerParam.jobId);
            IJobHandler jobHandler = jobThread != null ? jobThread.getHandler() : null;
            String removeOldReason = null;
            switch (triggerParam.glueType)
            {
                case "BEAN":
                    {
                        IJobHandler newJobHandler = XxlJobExecutor.loadJobHandler(triggerParam.executorHandler);

                        // valid old jobThread
                        if (jobThread != null && jobHandler != newJobHandler)
                        {
                            // change handler, need kill old thread
                            removeOldReason = "更换JobHandler或更换任务模式,终止旧任务线程";

                            jobThread = null;
                            jobHandler = null;
                        }

                        // valid handler
                        if (jobHandler == null)
                        {
                            jobHandler = newJobHandler;
                            if (jobHandler == null)
                            {
                                return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "job handler [" + triggerParam.executorHandler + "] not found.");
                            }
                        }
                        break;
                    }
                default:
                    {
                        return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "glueType[" + triggerParam.glueType + "] is not valid.");
                    }
            }



            //// executor block strategy
            if (jobThread != null)
            {

                ExecutorBlockStrategyEnum blockStrategy = (ExecutorBlockStrategyEnum)Enum.Parse(typeof(ExecutorBlockStrategyEnum), triggerParam.executorBlockStrategy);
                if (ExecutorBlockStrategyEnum.DISCARD_LATER == blockStrategy)
                {
                    // discard when running
                    if (jobThread.isRunningOrHasQueue())
                    {
                        return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "阻塞处理策略-生效：" + ExecutorBlockStrategyEnum.DISCARD_LATER.ToString());
                    }
                }
                else if (ExecutorBlockStrategyEnum.COVER_EARLY == blockStrategy)
                {
                    // kill running jobThread
                    if (jobThread.isRunningOrHasQueue())
                    {
                        removeOldReason = "阻塞处理策略-生效：" + ExecutorBlockStrategyEnum.COVER_EARLY.ToString();

                        jobThread = null;
                    }
                }
                else
                {
                    // just queue trigger
                }

            }

            // replace thread (new or exists invalid)
            if (jobThread == null)
            {
                jobThread = XxlJobExecutor.registJobThread(triggerParam.jobId, jobHandler, removeOldReason);
            }

            // push data to queue
            ReturnT<String> pushResult = jobThread.pushTriggerQueue(triggerParam);
            return pushResult;
        }
    }
}
